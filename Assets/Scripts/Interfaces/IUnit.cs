using Units;
using UnityEngine;

public interface IUnit
{
    public int Cost();
    public WaitForSeconds SpawnDelay();
    public UnitAbstract Prefab(bool isAlly);
}