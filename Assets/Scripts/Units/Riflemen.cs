using UnityEngine;

namespace Units
{
    public class Riflemen : UnitAbstract
    {
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