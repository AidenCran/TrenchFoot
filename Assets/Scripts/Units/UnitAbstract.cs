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
        [HideInInspector] public UnityEvent OnDeath;
        
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

        public bool IsDead => Stats.IsDead;

        public UnitStates state = UnitStates.Walking;
        
        [SerializeField] Animator animator;

        readonly int _shoot = Animator.StringToHash("ShootSolo");
        readonly int _climbUpProper = Animator.StringToHash("ClimbSolo");
        readonly int _death = Animator.StringToHash("DeathSolo");
        readonly int _drop = Animator.StringToHash("DropSolo");

        Vector2 _unitAttackRange;

        public SpriteRenderer GetRenderer { get; private set; }

        AudioSource _audioSource;

        public float hitChance = 90;
        
        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _rb = GetComponent<Rigidbody2D>();
            animator = GetComponentInChildren<Animator>();
            GetRenderer = GetComponentInChildren<SpriteRenderer>();

            state = UnitStates.Invincible;
        }

        protected virtual void Start()
        {
            Stats = new TempEntityStats
            {
                MaxHealth = 10,
                CurrentHealth = 15,
                MoveSpeed = 1.5f,
                Damage = 5,
                Range = 5f,
            };

            if (animator == null) print("Animator Null");

            _unitAttackRange = new Vector2(Stats.Range, 10);
            transform.position = spawnPoint;

            StartCoroutine(IFrames());
            IEnumerator IFrames()
            {
                yield return Helper.GetWait(5f);
                state = UnitStates.Walking;
            }
        }

        void Update() => MoveUnit();

        bool CanWalk()
        {
            if (state == UnitStates.Dead) return false;
            if (IsEnemyInRange()) return false;
            if (isInTrench) return false;

            return true;
        }

        void MoveUnit()
        {
            if (!CanWalk())
            {
                _rb.velocity = Vector2.zero;
                return;
            }

            var direction = isAlly ? Vector2.right : Vector2.left;

            // Else Move Forward
            _rb.velocity = new Vector2(direction.x * Stats.MoveSpeed, 0);
        }

        bool IsEnemyInRange()
        {
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

        /// <summary>
        /// Return Nullable Unit & Bool if null
        /// </summary>
        /// <returns></returns>
        (UnitAbstract, bool) UnitInRange()
        {
            var x = Physics2D.OverlapBox(transform.position, _unitAttackRange, 0, oppositionLayerMask);
            if (x == null) return (null, false);
            
            x.TryGetComponent<UnitAbstract>(out var unit);
            return unit.state == UnitStates.Dead ? (null, false) : (unit, true);
        }

        public void TakeDamage(float damage)
        {
            var x = UnityEngine.Random.Range(0, 100);
            if (x > hitChance)
            {
                // Miss
                // Play Sound??
                return;
            }
            
            // Else
            
            // Damage Entity
            // Return Is Dead
            // If Is Dead = Kill Unit
            if (Stats.DamageEntity(damage)) KillUnit();
        }

        void KillUnit()
        {
            if (state == UnitStates.Invincible) return;
            
            state = UnitStates.Dead;
            animator.CrossFade(_death, 0, 0);
            
            PlaySound(UnitSounds.DieSound);

            OnDeath?.Invoke();
            
            StartCoroutine(Wait());
            IEnumerator Wait()
            {
                yield return Helper.GetWait(5);
                ReleaseUnit();
            }
        }

        void ShootUnit(UnitAbstract unit)
        {
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

        public void ReleaseUnit()
        {
            GetRenderer.DOFade(0, 2).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
        
        void PlaySound(AudioClip clip)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
        }

        public void EnterTrenchAnim() => animator.CrossFade(_drop, 0, 0);
        public void ExitTrenchAnim() => animator.CrossFade(_climbUpProper, 0, 0);

        void OnDrawGizmosSelected() => Gizmos.DrawWireCube(transform.position, _unitAttackRange);
    }
}
