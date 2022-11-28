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
        [HideInInspector] public UnityEvent<UnitAbstract> OnDeath;
        [HideInInspector] public Action OnRelease;
        
        protected TempEntityStats Stats;
        
        // TODO: Hit Chance = Lower It In Trench
        
        public Vector3 spawnPoint;
        public LayerMask oppositionLayerMask;
        
        public bool isAlly;
        public bool isInTrench;
        public bool isAttackingTrench;

        Rigidbody2D _rb;

        bool _isCooldownActive;
        const float ReloadDelay = 0.5f;
        public float hitChance = 75;


        public UnitStates state = UnitStates.Walking;
        public bool IsDead => state == UnitStates.Dead;
        
        [SerializeField] Animator animator;

        readonly int _idle = Animator.StringToHash("WalkSolo");
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
            
            Stats = new TempEntityStats
            {
                CurrentHealth = 15,
                MoveSpeed = .75f,
                Damage = 5,
                Range = 5f,
            };
            
            state = UnitStates.Walking;

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
            animator.CrossFade(_idle, 0, 0);
            GetRenderer.DOFade(1, 0);
        }

        void Update() => MoveUnit();

        bool CanWalk()
        {
            if (state == UnitStates.Dead) return false;
            if (IsEnemyInRange()) return false;

            if (state == UnitStates.InTrench) return false;
            if (isAttackingTrench) return false;
            if (isInTrench) return false;

            if (state == UnitStates.Invincible) return true;

            return true;
        }

        /// <summary>
        /// Move Unit Horizontally
        /// </summary>
        void MoveUnit()
        {
            // Prevent Walk & Attack Check / Attack
            if (_pauseMenu.IsPaused)
            {
                _rb.velocity = Vector2.zero;
                return;
            }
            
            if (!CanWalk())
            {
                _rb.velocity = Vector2.zero;
                return;
            }
            
            animator.CrossFade(_idle, 0, 0);

            // Select Direction
            var direction = isAlly ? Vector2.right : Vector2.left;
            
            // Set Velocity
            _rb.velocity = new Vector2(direction.x * Stats.MoveSpeed, 0);
        }

        /// <summary>
        /// Checking if enemies are in range
        /// </summary>
        /// <returns></returns>
        bool IsEnemyInRange()
        {
            if (state == UnitStates.Dead) return false;
            
            // Get Unit
            var x = UnitInRange();
        
            // Does Unit Exist?
            if (!x.Item2) return false;

            // Await To Shoot
            if (_isCooldownActive || x.Item1.state == UnitStates.Invincible) return true;
            
            // Shoot
            ShootUnit(x.Item1);
            return true;
        }

        Vector2 AttackPoint()
        {
            var pos = transform.position;
            var offset = _unitAttackRange.x / 3;
            var result = isAlly ? pos.x += offset : pos.x -= offset;
            pos.x = result;
            return pos;
        }

        /// <summary>
        /// Raycast In-front of Unit (Square)
        /// Return Nullable Unit & Bool if null
        /// </summary>
        /// <returns></returns>
        (UnitAbstract, bool) UnitInRange()
        {
            var x = Physics2D.OverlapBox(AttackPoint(), _unitAttackRange, 0, oppositionLayerMask);
            if (x == null) return (null, false);
            
            x.TryGetComponent<UnitAbstract>(out var unit);
            if (unit.isAlly == isAlly) return (null, false);
            // If Unit Is Dead | Return False
            return unit.state == UnitStates.Dead ? (null, false) : (unit, true);
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
            
            state = UnitStates.Dead;
            animator.CrossFade(_death, 0, 0);
            
            PlaySound(UnitSounds.DieSound);

            OnDeath?.Invoke(this);
            
            print("I Died");
            
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
            state = UnitStates.Attacking;
            unit.TakeDamage(Stats.Damage);

            // Play Animation
            animator.CrossFade(_shoot, 0, 0);
            
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

        public void ReleaseUnit() => GetRenderer.DOFade(0, 2).OnComplete(() => Destroy(gameObject));

        void PlaySound(AudioClip clip) => _audioSource.PlayOneShot(clip);

        public void EnterTrenchAnim()
        {
            state = UnitStates.InTrench;
            animator.CrossFade(_drop, 0, 0);
        }

        public void ExitTrenchAnim()
        {
            animator.CrossFade(_climbUpProper, 0, 0);
            state = UnitStates.Walking;
        }

        void OnDrawGizmosSelected() => Gizmos.DrawWireCube(AttackPoint(), _unitAttackRange);
    }
}
