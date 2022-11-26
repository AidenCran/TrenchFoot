using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entities
{
    public class SpawnUnits : MonoBehaviour
    {
        [SerializeField] UnitAbstract enemyPrefab;
        [SerializeField] UnitAbstract allyPrefab;

        [SerializeField] Transform allyParent;
        [SerializeField] Transform enemyParent;

        [SerializeField] LayerMask allyMask;
        [SerializeField] LayerMask enemyMask;

        [SerializeField] float spawnHeight = 3;

        readonly WaitForSeconds _spawnCooldown = new(3);
        bool _isEnemyPrefabNull;
        bool _isAllyPrefabNull;

        void Start()
        {
            _isEnemyPrefabNull = enemyPrefab == null;
            _isAllyPrefabNull = allyPrefab == null;
            SpawnAlly();
            SpawnEnemies();
        }

        void SpawnEnemies()
        {
            StartCoroutine(SpawnEnemies());
            IEnumerator SpawnEnemies()
            {
                yield return _spawnCooldown;
                SpawnEnemy();
                
                // Repeat (Could Change To While Loop)
                StartCoroutine(SpawnEnemies());
            }
        }


        float GetSpawnPointVariation() => Random.Range(-spawnHeight, spawnHeight);


        // Really Bad Code
        // Do not repeat
        // Haha haha haha...

        bool canAllySpawn = true;
        
        void StartAllySpawnDelay()
        {
            canAllySpawn = false;
            StartCoroutine(AllySpawnDelay());
            IEnumerator AllySpawnDelay()
            {
                yield return _spawnCooldown;
                canAllySpawn = true;
            }
        }

        public void SpawnAlly()
        {
            if (_isAllyPrefabNull) return;
            if (!canAllySpawn) return;
            var x = Instantiate(allyPrefab, null);
            var spawnPosition = allyParent.position;
            spawnPosition.y += GetSpawnPointVariation();
            
            x.spawnPoint = spawnPosition;
            x.oppositionLayerMask = enemyMask;

            StartAllySpawnDelay();
        }

        void SpawnEnemy()
        {
            if (_isEnemyPrefabNull) return;
            var x = Instantiate(enemyPrefab, null);
            var spawnPosition = enemyParent.position;
            spawnPosition.y += GetSpawnPointVariation();
            
            x.spawnPoint = spawnPosition;
            x.oppositionLayerMask = allyMask;
            x.isMovingLeft = true;
        }
        

        void OnDrawGizmosSelected()
        {
            var bot = transform.position;
            bot.y -= spawnHeight;
        
            var top = transform.position;
            top.y += spawnHeight;
            
            Gizmos.DrawLine(bot, top);
        }
    }
}