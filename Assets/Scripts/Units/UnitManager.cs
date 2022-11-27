using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Units
{
    public class UnitManager : MonoBehaviour
    {
        #region Singleton

        public static UnitManager Instance { get; private set; }

        private void Singleton()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        #endregion

        [SerializeField] UnitAbstract AllyPrefab;
        [SerializeField] UnitAbstract EnemyPrefab;
        
        ObjectPool<UnitAbstract> AllyPool;
        ObjectPool<UnitAbstract> EnemyPool;

        void Awake()
        {
            Singleton();
        }

        void Start()
        {
            AllyPool = new ObjectPool<UnitAbstract>(() =>
            {
                var x = Instantiate(AllyPrefab, null);
                x.gameObject.SetActive(false);
                return x;
            }, (x) =>
            {
                x.gameObject.SetActive(true);
                x.Init();
            }, (x) =>
            {
                x.gameObject.SetActive(false);
            }, (x) =>
            {
                Destroy(x.gameObject);
            });
            
            EnemyPool = new ObjectPool<UnitAbstract>(() => Instantiate(EnemyPrefab, null), (x) =>
            {
                x.gameObject.SetActive(true);
                x.Init();
            }, (x) =>
            {
                x.gameObject.SetActive(false);
            }, (x) =>
            {
                Destroy(x.gameObject);
            });
        }

        public UnitAbstract GetAllyUnit() => AllyPool.Get();

        public UnitAbstract GetEnemyUnit() => EnemyPool.Get();

        public void ReleaseAllyUnit(UnitAbstract unit) => AllyPool.Release(unit);

        public void ReleaseEnemyUnit(UnitAbstract unit) => EnemyPool.Release(unit);
    }
}