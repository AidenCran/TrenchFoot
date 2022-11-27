using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DG.Tweening;
using Units;
using UnityEngine;
using Utility;

[RequireComponent(typeof(BoxCollider2D))]
public class Trench : MonoBehaviour
{
    // TODO: Units Enter Exit Trench
    readonly List<UnitAbstract> _unitsInTrench = new();

    const int MaxTrenchUnitCount = 6;
    const float Delay = .25f;

    int _countInTrench = 0;

    HelperMonoBehaviour _helperMonoBehaviour;

    [SerializeField] bool isAllyTrench;
    
    const float Duration = 0.5f;

    bool IsTrenchEmpty => _countInTrench == 0;

    float _trenchHitReduction = 50f;

    void Start() => _helperMonoBehaviour = HelperMonoBehaviour.Instance;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.TryGetComponent<UnitAbstract>(out var unit)) return;

        // if (unit.isAlly != _unitsInTrench[0].isAlly)
        // {
        //     
        // }
        
        EnterTrench(unit);
    }

    // PROBLEMS
    // Enemies not staying in trench after it filled???!?

    void EnterTrench(UnitAbstract unit)
    {
        _unitsInTrench.Add(unit);
        _countInTrench++;
        
        unit.isInTrench = true;
        unit.hitChance -= _trenchHitReduction;
        
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

    void ExitTrench(UnitAbstract unit)
    {
        _unitsInTrench.Remove(unit);
        _countInTrench--;
        
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

    void SetUnitLayers()
    {
        var arr = _unitsInTrench.OrderBy((x) => x.transform.position.y).Reverse().ToArray();
        for (var i = 0; i < arr.Length; i++)
        {
            arr[i].GetRenderer.sortingOrder = 100 + i;
        }
    }

    // TODO: Purge (All Units Charge Out Of Trench)
    void Purge()
    {
        foreach (var unit in _unitsInTrench)
        {
            unit.isInTrench = false;
        }
    }
    
    // TODO: Units Gain Cover?? % Hit Chance
    
    // TODO: Take Over Enemy Trenches?

    // TODO: Able To Buy / Place Multiple Trenches (Cap At 3)
    // OTHER FILE ^*
}