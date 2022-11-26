using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StatType
{
    Health,
    Speed,
    Damage,
    XPGain
}

public struct TempEntityStats
{
    public TempEntityStats(float maxHP, float moveSpeed, float damage)
    {
        MaxHealth = maxHP;
        MoveSpeed = moveSpeed;
        Damage = damage;

        CurrentHealth = MaxHealth;

        M_HPUpgradeVal = 0;
        MSUpgradeVal = 0;
        DMGUpgradeVal = 0;
    }

    public TempEntityStats(EntityStats stats)
    {
        MaxHealth = stats.MaxHealth;
        MoveSpeed = stats.MoveSpeed;
        Damage = stats.Damage;

        CurrentHealth = MaxHealth;

        M_HPUpgradeVal = 0;
        MSUpgradeVal = 0;
        DMGUpgradeVal = 0;
    }

    public float MaxHealth;
    public float CurrentHealth;
    public float MoveSpeed;
    public float Damage;

    public int M_HPUpgradeVal;
    public int MSUpgradeVal;
    public int DMGUpgradeVal;

    public bool IsDead => CurrentHealth <= 0;

    public void ModifyStats(StatType statType, float value, bool isPercentage, bool bypassRestriction = false)
    {
        if (isPercentage) value = value / 100;

        switch (statType)
        {
            case StatType.Health:
                if (M_HPUpgradeVal >= 5 && !bypassRestriction) return;
                if (isPercentage)
                {
                    MaxHealth += MaxHealth * value;
                    CurrentHealth += MaxHealth * value;
                }
                else
                {
                    MaxHealth += value;
                    CurrentHealth += value;
                }

                Mathf.Clamp(CurrentHealth, 0, MaxHealth);

                if (!bypassRestriction) M_HPUpgradeVal++;
                break;
            case StatType.Speed
            :
                if (MSUpgradeVal >= 5 && !bypassRestriction) return;
                if (isPercentage) MoveSpeed += MoveSpeed * value;
                else MoveSpeed += value;

                if (!bypassRestriction) MSUpgradeVal++;
                break;
            case StatType.Damage: 
                if (DMGUpgradeVal >= 5 && !bypassRestriction) return;
                if (isPercentage) Damage += Damage * value;
                else Damage += value;
                if (!bypassRestriction) DMGUpgradeVal++;
                break;
            default:
                break;
        }
    }
}

[CreateAssetMenu(menuName = "Custom Assets/Stats")]
public class EntityStats : ScriptableObject
{
    [SerializeField] float _maxHealth = 10;
    [SerializeField] float _moveSpeed = 5;
    [SerializeField] float _damage = 5;

    public float MaxHealth => _maxHealth;
    public float MoveSpeed => _moveSpeed;
    public float Damage => _damage;
}