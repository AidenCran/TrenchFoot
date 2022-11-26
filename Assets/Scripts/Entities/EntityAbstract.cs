using System.Collections;
using UnityEngine;

public class EntityAbstract : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] bool isGoingLeft;
    
    [SerializeField] GameObject prefab;

    TempEntityStats _stats;
    
    protected void Start() => SpawnEntity();

    void TakeDamage(float damage)
    {
        if (_stats.IsDead)
        {
            KillEntity();
            return;
        }
        
        _stats.ModifyStats(StatType.Health, damage, false);
    }

    void SpawnEntity()
    {
        if (prefab == null) return;
        
        var x = Instantiate(prefab, transform);
        x.transform.position = spawnPoint.position;
        
        _stats = new TempEntityStats();
    }

    void KillEntity()
    {
        // Temp
        gameObject.SetActive(false);
    }

    void Shoot(EntityAbstract entity)
    {
        // Shoot
        entity.TakeDamage(_stats.Damage);
        
        // Reload
        Reload();
    }

    void Reload()
    {
        StartCoroutine(Reload());
        IEnumerator Reload()
        {
            yield return null;
        }
    }
}
