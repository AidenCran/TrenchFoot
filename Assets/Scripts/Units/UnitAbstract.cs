using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

namespace Units
{
    public abstract class UnitAbstract : MonoBehaviour, IDamageable
    {
        // ReSharper disable once UnassignedField.Global
        [NonSerialized] public UnityEvent<UnitAbstract> OnDeath;
        [field: HideInInspector] public Action OnRelease { get; set; }

        protected TempEntityStats Stats;
        
        // TODO: Hit Chance = Lower It In Trench
        
        public Vector3 spawnPoint;
        public LayerMask oppositionLayerMask;
        
        public bool isAlly;
        public bool isInTrench;
        public bool isAttackingTrench;
        public bool trenchCooldown;

        Rigidbody2D _rb;

        bool _isCooldownActive;
        const float ReloadDelay = 0.5f;
        public float hitChance = 75;

        public UnitStates state = UnitStates.Walking;
        public bool IsDead => state == UnitStates.Dead;

        [SerializeField] Animator animator;

        readonly int _walkAnim = Animator.StringToHash("WalkSolo");
        readonly int _shoot = Animator.StringToHash("ShootSolo");
        readonly int _climbUpProper = Animator.StringToHash("ClimbSolo");
        readonly int _death = Animator.StringToHash("DeathSolo");
        readonly int _drop = Animator.StringToHash("DropSolo");

        Vector2 _unitAttackRange;

        public SpriteRenderer GetRenderer { get; private set; }

        AudioSource _audioSource;

        PauseMenu _pauseMenu;

        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _rb = GetComponent<Rigidbody2D>();
            animator = GetComponentInChildren<Animator>();
            GetRenderer = GetComponentInChildren<SpriteRenderer>();

            Stats = new TempEntityStats(0.25f, 5, 5, 15);

            isInTrench = false;
            isAttackingTrench = false;

            _unitAttackRange = new Vector2(Stats.Range, 10);
        }

        protected virtual void Start()
        {
            _pauseMenu = PauseMenu.instance;
            Init();
        }

        void Init()
        {
            SetAnimState(_walkAnim, UnitStates.Walking);
            GetRenderer.DOFade(1, 0);
        }

        void Update()
        {
            if (_pauseMenu.IsPaused)
            {
                _rb.velocity = Vector2.zero;
                return;
            }
            
            if (IsDead) return;
            if (IsEnemyInRange()) return;
            
            MoveUnit();
        }

        bool CanWalk()
        {
            if (state == UnitStates.InTrench) return false;
            if (isAttackingTrench) return false;
            if (isInTrench) return false;
            return true;
        }

        /// <summary>
        /// Move Unit Horizontally
        /// </summary>
        void MoveUnit()
        {
            if (!CanWalk())
            {
                StopMoving();
                return;
            }
            
            SetAnimState(_walkAnim, UnitStates.Walking);

            // Select Direction
            var direction = isAlly ? 1 : -1;
            
            // Set Velocity
            _rb.velocity = new Vector2(direction * Stats.MoveSpeed, 0);
        }

        /// <summary>
        /// Checking if enemies are in range
        /// </summary>
        /// <returns></returns>
        bool IsEnemyInRange()
        {
            // Get Unit
            var x = UnitInRange();
        
            // Does Unit Exist?
            if (!x.Item2) return false;

            // Await To Shoot
            if (_isCooldownActive) return true;
            
            // Shoot
            ShootUnit(x.Item1);
            return true;
        }

        [SerializeField] bool DebugAttack;
        
        (UnitAbstract, bool) UnitInRange()
        {
            if (DebugAttack) print("Checking To Attack");
            
            // Raycast For Unit
            var x = Physics2D.OverlapBoxAll(transform.position, _unitAttackRange, 0, oppositionLayerMask);

            // If Null
            if (x == null) return (null, false);
            if (x.Length == 0) return (null, false);
            if (!x.SelectRandom().TryGetComponent<UnitAbstract>(out var unit)) return (null, false);
            if (unit.isAlly == isAlly || unit.state == UnitStates.Dead) return (null, false);
            
            return (unit, true);
        }

        public void TakeDamage(float damage)
        {
            var x = UnityEngine.Random.Range(0, 100);
            if (x > hitChance)
            {
                // Miss
                // Play Sound
                PlaySound(UnitSounds.MissedShot);
                return;
            }

            // Damage Entity
            // Return Is Dead
            // If Is Dead = Kill Unit
            if (Stats.DamageEntity(damage)) KillUnit();
        }

        void KillUnit()
        {
            // If Already Dead
            if (state == UnitStates.Dead) return;
            
            SetAnimState(_death, UnitStates.Dead);
            StopMoving();

            PlaySound(UnitSounds.DieSound);

            OnDeath?.Invoke(this);

            StartCoroutine(Wait());
            IEnumerator Wait()
            {
                yield return Helper.GetWait(5);
                ReleaseUnit();
            }
        }

        void ShootUnit(UnitAbstract unit)
        {
            if (state == UnitStates.Dead) return;
            if (unit.state == UnitStates.Dead) return;

            // Shoot
            unit.TakeDamage(Stats.Damage);

            // Play Animation
            SetAnimState(_shoot, UnitStates.Attacking);
            StopMoving();
            
            // Play Sound
            PlaySound(UnitSounds.ShootSound);
            
            // Reload
            Reload();
        }

        void Reload()
        {
            _isCooldownActive = true;
            StartCoroutine(LocalReload());
            IEnumerator LocalReload()
            {
                yield return Helper.GetWait(ReloadDelay);
                _isCooldownActive = false;
            }
        }

        void StopMoving() => _rb.velocity = Vector2.zero;

        public void ReleaseUnit() => GetRenderer.DOFade(0, 2).OnComplete(() => Destroy(gameObject));

        void PlaySound(AudioClip clip) => _audioSource.PlayOneShot(clip);

        #region Anims

        void SetAnimState(int animState, UnitStates state)
        {
            animator.CrossFade(animState, 0, 0);
            this.state = state;
        }

        public void EnterTrenchAnim() => SetAnimState(_drop, UnitStates.InTrench);

        public void ExitTrenchAnim() => SetAnimState(_climbUpProper, UnitStates.Walking);

        #endregion 
        
        void OnDrawGizmos() => Gizmos.DrawWireCube(transform.position, _unitAttackRange);
    }
}
