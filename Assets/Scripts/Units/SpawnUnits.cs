using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Units
{
    public class SpawnUnits : MonoBehaviour
    {
        // Further Refactor
        // Two Spawners | One For Each Side
        // Spawner Is The Side's Parent | Makes Below Transform Useless
        [SerializeField] Transform allyParent;
        [SerializeField] Transform enemyParent;

        // Each Spawner Contains Opposition LayerMask
        // Means Only 1 Mask Required
        [SerializeField] LayerMask allyMask;
        [SerializeField] LayerMask enemyMask;

        // Spawn Height Stays
        [SerializeField] float spawnHeight = 3;

        // TODO: Change To UI Script
        // UI Extracted To UI Script
        [SerializeField] Button buyRiflemanButton;

        readonly WaitForSeconds _enemyAdditionalDelay = new(2);

        readonly Dictionary<UnitType, IUnit> _unitDict = new();

        bool _canAllySpawn = true;

        // Score Manager Stays
        ScoreManager _scoreManager;

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

                SpawnUnit(_unitDict[UnitType.Rifleman], true);
                _scoreManager.ReduceScore(x.Cost());
            });

            SpawnUnit(_unitDict[UnitType.Rifleman], true);
            AutoSpawnEnemies(_unitDict[UnitType.Rifleman]);
        }

        float GetSpawnPointVariation()
        {
            return Random.Range(-spawnHeight / 2, spawnHeight / 2);
        }

        void AutoSpawnEnemies(IUnit unit)
        {
            StartCoroutine(AutoSpawn());

            IEnumerator AutoSpawn()
            {
                yield return unit.SpawnDelay();
                yield return _enemyAdditionalDelay;

                SpawnUnit(unit, false);

                // Repeat (Could Change To While Loop)
                StartCoroutine(AutoSpawn());
            }
        }

        // TODO: Change From Unit Spawn Delay To Static 0.5s?
        void StartAllySpawnDelay(IUnit unit)
        {
            _canAllySpawn = false;
            StartCoroutine(AllySpawnDelay());

            IEnumerator AllySpawnDelay()
            {
                yield return Helper.GetWait(0.5f);
                _canAllySpawn = true;
            }
        }

        void SpawnUnit(IUnit unit, bool isAlly)
        {
            var x = Instantiate(unit.Prefab(isAlly), null);

            // If Ally => Ally Parent
            var spawnPosition = isAlly ? allyParent.position : enemyParent.position;
            spawnPosition.y += GetSpawnPointVariation();

            x.spawnPoint = spawnPosition;

            // If Ally => Enemy Mask is Opposition
            x.oppositionLayerMask = isAlly ? enemyMask : allyMask;

            // If Ally => Moving Right
            x.isAlly = isAlly;

            if (isAlly) StartAllySpawnDelay(unit);
        }
        
        void OnDrawGizmosSelected()
        {
            var position = transform.position;
            var bot = position;
            bot.y -= spawnHeight / 2;
            
            var top = position;
            top.y += spawnHeight / 2;
            
            Gizmos.DrawLine(bot, top);
        }
    }
}