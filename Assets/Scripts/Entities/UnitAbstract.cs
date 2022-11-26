using System.Collections;
using DefaultNamespace;
using UnityEngine;

namespace Entities
{
    public abstract class UnitAbstract : MonoBehaviour, IDamageable
    {
        // TODO: Hit Chance = Lower It In Trench
        
        public Vector3 spawnPoint;
        public LayerMask oppositionLayerMask;
        
        public bool isMovingLeft;
    
        TempEntityStats _stats;

        Rigidbody2D _rb;

        bool _isCooldownActive;
        
        readonly WaitForSeconds _reloadDelay = new(0.5f);

        protected void Start()
        {
            _stats = new TempEntityStats
            {
                MaxHealth = 10,
                CurrentHealth = 10,
                MoveSpeed = 3,
                Damage = 5,
                Range = 3f,
            };
            
            transform.position = spawnPoint;
            _rb = GetComponent<Rigidbody2D>();
        }

        void Update() => MoveUnit();

        void MoveUnit()
        {
            if (IsEnemyInRange())
            {
                _rb.velocity = Vector2.zero;
                return;
            }

            Vector2 direction;
            if (isMovingLeft) direction = Vector2.left;
            else direction = Vector2.right;

            // Else Move Forward
            _rb.velocity = new Vector2(direction.x * _stats.MoveSpeed, 0);
        }

        bool IsEnemyInRange()
        {
            // Get Unit
            var x = UnitInRange();
        
            // Does Unit Exist?
            if (!x.Item2) return false;

            // Await To Shoot
            if (_isCooldownActive) return true;
        
            // Shoot
            Shoot(x.Item1);
            return true;
        }

        /// <summary>
        /// Return Nullable Unit & Bool if null
        /// </summary>
        /// <returns></returns>
        (UnitAbstract, bool) UnitInRange()
        {
            var size = new Vector2(1.25f, _stats.Range);
            var x = Physics2D.OverlapBox(transform.position, size, 0, oppositionLayerMask);
            if (x == null) return (null, false);
            
            x.TryGetComponent<UnitAbstract>(out var unit);
            return (unit, true);
        }

        public void TakeDamage(float damage)
        {
            // Damage Entity
            // Return Is Dead
            // If Is Dead = Kill Unit
            if (_stats.DamageEntity(damage)) KillUnit();
        }

        void KillUnit()
        {
            // Temp
            gameObject.SetActive(false);
        }

        void Shoot(UnitAbstract unit)
        {
            // Shoot
            unit.TakeDamage(_stats.Damage);
        
            // Reload
            Reload();
        }

        void Reload()
        {
            _isCooldownActive = true;
            StartCoroutine(Reload());
            IEnumerator Reload()
            {
                yield return _reloadDelay;
                _isCooldownActive = false;
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, _stats.Range);   
        }
    }
}
