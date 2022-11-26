using DefaultNamespace;
using UnityEngine;

public class BaseTower : MonoBehaviour, IDamageable
{
    public float HPRemaining = 5;

    [SerializeField] bool isAlly = true;
    
    public void TakeDamage(float damage = 1)
    {
        HPRemaining -= damage;
        print($"Damage Taken: {isAlly} {HPRemaining} HP Remaining");
        if (HPRemaining <= 0) DestroyTower();
    }

    void DestroyTower()
    {
        switch (isAlly)
        {
            case true:
                print("Allies Loose");
                break;
            case false:
                print("Allies WIN!!!");
                break;
        }
    }
}
