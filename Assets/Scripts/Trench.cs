using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DG.Tweening;
using Units;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utility;

[RequireComponent(typeof(BoxCollider2D))]
public class Trench : MonoBehaviour
{
    // TODO: Units Enter Exit Trench
    [SerializeField] List<UnitAbstract> _unitsInTrench = new();

    const int MaxTrenchUnitCount = 6;
    const float Delay = .25f;

    int _countInTrench = 0;

    HelperMonoBehaviour _helperMonoBehaviour;

    [SerializeField] bool isAllyTrench;
    
    const float Duration = 0.5f;

    [SerializeField] bool IsTrenchEmpty => _countInTrench == 0;

    const float _trenchHitReduction = 15f;

    [SerializeField] List<UnitAbstract> _awaitingToEnterTrench = new();

    [SerializeField] Button _chargeButton;
    
    bool _purgeCooldownActive;

    void Start()
    {
        _helperMonoBehaviour = HelperMonoBehaviour.Instance;
        _chargeButton.interactable = isAllyTrench;
        _chargeButton.onClick.AddListener(Purge);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.TryGetComponent<UnitAbstract>(out var unit)) return;

        if (_unitsInTrench.Count == 0)
        {
            EnterTrench(unit);
            return;
        }

        if (unit.isAlly == _unitsInTrench[0].isAlly)
        {
            EnterTrench(unit);
        }
        
        if (unit.isAlly != _unitsInTrench[0].isAlly)
        {
            // Await Entering Trench
            unit.isAttackingTrench = true;
            _awaitingToEnterTrench.Add(unit);
            return;
        }
    }

    void EnterTrench(UnitAbstract unit)
    {
        if (_unitsInTrench.Contains(unit)) return;

        _unitsInTrench = _unitsInTrench.Where((x) => x != null).ToList();
        
        _unitsInTrench.Add(unit);
        _countInTrench++;
        
        unit.isInTrench = true;
        unit.hitChance -= _trenchHitReduction;

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
            if (_countInTrench < MaxTrenchUnitCount) return;
            ExitTrench(unit);
        });
    }

    void CheckIfTrenchIsEmpty()
    {
        if (_unitsInTrench.Count != 0) return;
        if (_awaitingToEnterTrench.Count <= 0) return;
        
        _unitsInTrench.Clear();
        
        isAllyTrench = _awaitingToEnterTrench[0].isAlly;
        _chargeButton.interactable = isAllyTrench;
        
        foreach (var unit in _awaitingToEnterTrench)
        {
            unit.isAttackingTrench = false;
            EnterTrench(unit);
        }
    }

    // BUG: Units Aren't Always Removed From List But Play As Normal...
    void ExitTrench(UnitAbstract unit)
    {
        RemoveUnit(unit);

        if (unit.state == UnitStates.Dead) return;

        // Tween Unit Out Of Trench
        var newPos = unit.transform.position;
        newPos.x += unit.isAlly ? 1f : -1f;
        newPos.y += unit.GetRenderer.size.y / 2;
        unit.transform.DOMove(newPos, Duration);
        
        // Play Exit Anim
        unit.ExitTrenchAnim();
        
        // After Anim
        _helperMonoBehaviour.InvokeActionOnComplete(() =>
        {
            unit.isInTrench = false;
            unit.hitChance += _trenchHitReduction;
        }, Delay);
    }
    
    void RemoveUnit(UnitAbstract unit)
    {
        _unitsInTrench.Remove(unit);
        unit.OnDeath?.RemoveListener(RemoveUnit);
        _countInTrench--;
        CheckIfTrenchIsEmpty();
    }

    // Sprite Layering
    void SetUnitLayers()
    {
        var arr = _unitsInTrench.Where((x) => x != null && !x.IsDead).OrderBy((x) => x.transform.position.y).Reverse().ToArray();
        for (var i = 0; i < arr.Length; i++)
        {
            arr[i].GetRenderer.sortingOrder = 100 + i;
        }
    }
    
    // All Units Charge Out Of Trench
    void Purge()
    {
        if (_purgeCooldownActive) return;
        if (_unitsInTrench == null) return;
        if (_unitsInTrench.Count == 0) return;

        PurgeCooldown();
        
        for (var i = 0; i < _unitsInTrench.Count; i++)
        {
            var unit = _unitsInTrench[i];
            if (unit == null) continue;
            ExitTrench(unit);
        }
    }

    void PurgeCooldown()
    {
        StartCoroutine(Cooldown());
        IEnumerator Cooldown()
        {
            _purgeCooldownActive = true;
            _chargeButton.interactable = false;
            yield return Helper.GetWait(5);
            
            _purgeCooldownActive = false;

            SetPurgeAccess();
        }
    }

    void SetPurgeAccess()
    {
        if (_purgeCooldownActive) return;
        if (_unitsInTrench.Count == 0) return;
        
        _chargeButton.interactable = _unitsInTrench[0].isAlly;
    }
}