using UnityEngine;

namespace Units
{
    public class Riflemen : UnitAbstract
    {
        protected override void Start()
        {
            base.Start();
            
            Stats = new TempEntityStats
            {
                MaxHealth = 10,
                CurrentHealth = 15,
                MoveSpeed = 3,
                Damage = 5,
                Range = 3f,
            };
        }
    }

    /// Unit Data Record
    public record RiflemenRecord : IUnit
    {
        readonly int _cost = 10;
        readonly WaitForSeconds _spawnDelay = new(3);
        readonly UnitAbstract _allyPrefab = Resources.Load<UnitAbstract>("Prefabs/AllyRifleman");
        readonly UnitAbstract _enemyPrefab = Resources.Load<UnitAbstract>("Prefabs/EnemyRifleman");

        public int Cost() => _cost;

        public WaitForSeconds SpawnDelay() => _spawnDelay;

        public UnitAbstract Prefab(bool isAlly) => isAlly ? _allyPrefab : _enemyPrefab;
    }
}