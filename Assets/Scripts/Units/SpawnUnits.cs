using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Units
{
    public class SpawnUnits : MonoBehaviour
    {
        [SerializeField] Transform allyParent;
        [SerializeField] Transform enemyParent;

        [SerializeField] LayerMask allyMask;
        [SerializeField] LayerMask enemyMask;

        [SerializeField] float spawnHeight = 3;

        ScoreManager _scoreManager;

        // TODO: Change To UI Script
        [SerializeField] Button buyRiflemanButton;
        
        readonly Dictionary<UnitType, IUnit> _unitDict = new();
        
        readonly WaitForSeconds _enemyAdditionalDelay = new(2);

        void Start()
        {
            _unitDict.Add(UnitType.Rifleman, new RiflemenRecord());

            _scoreManager = ScoreManager.instance;
            ;
            buyRiflemanButton.onClick.AddListener(() =>
            {
                var x = _unitDict[UnitType.Rifleman];

                if (!_canAllySpawn) return;
                if (_scoreManager.Score < x.Cost()) return;

                SpawnAlly(_unitDict[UnitType.Rifleman]);
                _scoreManager.ReduceScore(x.Cost());
            });

            SpawnAlly(_unitDict[UnitType.Rifleman]);
            AutoSpawnEnemies(_unitDict[UnitType.Rifleman]);
        }

        void AutoSpawnEnemies(IUnit unit)
        {
            StartCoroutine(SpawnEnemies(unit));

            IEnumerator SpawnEnemies(IUnit unit1)
            {
                yield return unit.SpawnDelay();
                yield return _enemyAdditionalDelay;

                SpawnEnemy(unit);

                // Repeat (Could Change To While Loop)
                StartCoroutine(SpawnEnemies(unit));
            }
        }

        float GetSpawnPointVariation()
        {
            return Random.Range(-spawnHeight, spawnHeight);
        }

        bool _canAllySpawn = true;

        void StartAllySpawnDelay(IUnit unit)
        {
            _canAllySpawn = false;
            StartCoroutine(AllySpawnDelay());

            IEnumerator AllySpawnDelay()
            {
                yield return unit.SpawnDelay();
                _canAllySpawn = true;
            }
        }

        void SpawnAlly(IUnit unit)
        {
            if (!_canAllySpawn) return;
            var x = Instantiate(unit.Prefab(true), null);

            var spawnPosition = allyParent.position;
            spawnPosition.y += GetSpawnPointVariation();

            x.spawnPoint = spawnPosition;
            x.oppositionLayerMask = enemyMask;
            x.isMovingLeft = false;

            StartAllySpawnDelay(unit);
        }

        void SpawnEnemy(IUnit unit)
        {
            var x = Instantiate(unit.Prefab(false), null);

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