using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DG.Tweening;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utility;

[RequireComponent(typeof(BoxCollider2D))]
public class Trench : MonoBehaviour
{
    // TODO: Units Enter Exit Trench
    [SerializeField] List<UnitAbstract> unitsInTrench = new();

    const int MaxTrenchUnitCount = 6;
    const float Delay = .25f;

    HelperMonoBehaviour _helperMonoBehaviour;

    [SerializeField] bool isAllyTrench;
    
    const float Duration = 0.5f;

    [SerializeField] bool IsTrenchEmpty => unitsInTrench.Count == 0;

    const float TrenchHitReduction = 10f;

    [SerializeField] List<UnitAbstract> awaitingToEnterTrench = new();

    [SerializeField] Button chargeButton;
    [SerializeField] TMP_Text unitCount;
    
    bool _purgeCooldownActive;

    void Start()
    {
        _helperMonoBehaviour = HelperMonoBehaviour.Instance;
        chargeButton.interactable = isAllyTrench;
        chargeButton.onClick.AddListener(Purge);
    }

    void Update()
    {
        CheckIfTrenchIsEmpty();
        unitCount.text = $"{unitsInTrench.Count}/{MaxTrenchUnitCount-1}";
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.TryGetComponent<UnitAbstract>(out var unit)) return;
        TryEnterTrench(unit);
    }

    void TryEnterTrench(UnitAbstract unit)
    {
        FilterOutDeadUnits();
        
        if (IsTrenchEmpty || unit.isAlly == unitsInTrench[0].isAlly)
        {
            EnterTrench(unit);
            return;
        }

        // Else
        // Opposition Attempts To Enter Trench
        
        // Await Entering Trench
        if (awaitingToEnterTrench.Contains(unit)) return;
        awaitingToEnterTrench.Add(unit);
        unit.isAttackingTrench = true;

        CheckIfTrenchIsEmpty();
    }

    void FilterOutDeadUnits() => unitsInTrench = unitsInTrench.Where((x) => x != null && !x.IsDead).ToList();

    void EnterTrench(UnitAbstract unit)
    {
        // Just Exited Trench
        if (unit.trenchCooldown) return;
        if (unitsInTrench.Contains(unit)) return;

        unitsInTrench = unitsInTrench.Where((x) => x != null).ToList();
        
        unitsInTrench.Add(unit);

        unit.isInTrench = true;
        unit.hitChance -= TrenchHitReduction;

        unit.OnDeath?.AddListener(RemoveUnit);

        isAllyTrench = unit.isAlly;

        SetPurgeAccess();

        // Play Enter Anim
        unit.EnterTrenchAnim();
        SetUnitLayers();

        // Tween Unit Into Trench
        var newPos = unit.transform.position;
        newPos.x += unit.isAlly ? 2.2f : -2.2f;
        newPos.y -= unit.GetRenderer.size.y / 2;
        unit.transform.DOMove(newPos, Duration + 1).OnComplete(() =>
        {
            if (unitsInTrench.Count < MaxTrenchUnitCount) return;
            ExitTrench(unit);
        });
    }

    void CheckIfTrenchIsEmpty()
    {
        FilterOutDeadUnits();
        
        if (unitsInTrench.Count != 0) return;
        if (awaitingToEnterTrench.Count <= 0) return;
        
        SwapInAwaitingUnits();

        void SwapInAwaitingUnits()
        {
            unitsInTrench.Clear();

            isAllyTrench = awaitingToEnterTrench[0].isAlly;
            chargeButton.interactable = isAllyTrench;
            
            // Check Dead Bodies Too
            foreach (var unit in awaitingToEnterTrench.Where(unit => unit != null))
            {
                unit.isAttackingTrench = false;
                EnterTrench(unit);
            }

            awaitingToEnterTrench.Clear();
        }
    }

    // BUG: Units Aren't Always Removed From List But Play As Normal...
    void ExitTrench(UnitAbstract unit)
    {
        RemoveUnit(unit);

        if (unit.state == UnitStates.Dead) return;

        unit.trenchCooldown = true;
        
        // Tween Unit Out Of Trench
        var newPos = unit.transform.position;
        newPos.x += unit.isAlly ? 1f : -1f;
        newPos.y += unit.GetRenderer.size.y / 2;
        unit.transform.DOMove(newPos, Duration);
        
        unit.OnDeath?.RemoveListener(RemoveUnit);
        
        // Play Exit Anim
        unit.ExitTrenchAnim();
        
        
        // After Anim
        StartCoroutine(Delay());
        IEnumerator Delay()
        {
            yield return Helper.GetWait(0.25f);
            unit.isInTrench = false;
            unit.hitChance += TrenchHitReduction;
            yield return Helper.GetWait(0.5f);
            unit.trenchCooldown = false;
        }
    }
    
    void RemoveUnit(UnitAbstract unit)
    {
        unitsInTrench.Remove(unit);
        unit.OnDeath?.RemoveListener(RemoveUnit);
        CheckIfTrenchIsEmpty();
    }

    // Sprite Layering
    void SetUnitLayers()
    {
        var arr = unitsInTrench.Where((x) => x != null && !x.IsDead).OrderBy((x) => x.transform.position.y).Reverse().ToArray();
        for (var i = 0; i < arr.Length; i++)
        {
            arr[i].GetRenderer.sortingOrder = 100 + i;
        }
    }
    
    // All Units Charge Out Of Trench
    void Purge()
    {
        if (_purgeCooldownActive) return;
        if (unitsInTrench == null) return;
        if (unitsInTrench.Count == 0) return;

        PurgeCooldown();
        
        foreach (var unit in unitsInTrench.Where((x)=>x != null).ToList())
        {
            ExitTrench(unit);
        }
    }

    void PurgeCooldown()
    {
        StartCoroutine(Cooldown());
        IEnumerator Cooldown()
        {
            _purgeCooldownActive = true;
            chargeButton.interactable = false;
            yield return Helper.GetWait(5);
            
            _purgeCooldownActive = false;

            SetPurgeAccess();
        }
    }

    void SetPurgeAccess()
    {
        if (_purgeCooldownActive) return;
        if (unitsInTrench.Count == 0) return;
        
        chargeButton.interactable = unitsInTrench[0].isAlly;
    }
}