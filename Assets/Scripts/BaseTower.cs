using System;
using DefaultNamespace;
using Units;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BaseTower : MonoBehaviour, IDamageable
{
    public float HPRemaining = 5;

    [SerializeField] bool isAlly = true;

    void OnTriggerExit2D(Collider2D other)
    {
        // If Unit | Release | Take Damage
        if (other.TryGetComponent<UnitAbstract>(out var unit))
        {
            // Guard Against Self Attack
            if (unit.isAlly == isAlly) return;
            unit.ReleaseUnit();
            TakeDamage();
        }
    }

    public void TakeDamage(float damage = 1)
    {
        HPRemaining -= damage;
        if (HPRemaining <= 0) DestroyTower();
    }

    void DestroyTower()
    {
        switch (isAlly)
        {
            case true:
                // Lose Screen
                GameHandler.instance.LoseGame();
                break;
            case false:
                // Win Screen
                GameHandler.instance.WinGame();
                break;
        }
    }
}
