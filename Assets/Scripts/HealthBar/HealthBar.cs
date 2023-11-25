using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType
{
    player,
    enemy
}  
public class HealthBar : MonoBehaviour
{
    private Vector3 localScale;
    [SerializeField] private CharacterType characterType;

    // Start is called before the first frame update
    void Start()
    {
        localScale = transform.localScale;
    }

    public void UpdateHealthBar()
    {
        float currentHealth = GetCurrentHealth();
        float healthPercentage = currentHealth / GetMaxHealth();

        localScale.x = healthPercentage * localScale.x;
        transform.localScale = localScale;
    }

    private float GetCurrentHealth()
    {
        if (characterType == CharacterType.player)
        {
            return GetComponentInParent<PlayerController>().playerHealth;
        }
        return GetComponentInParent<EnemyController>().life;
    }

    private float GetMaxHealth()
    {
        if (characterType == CharacterType.player)
        {
            return GetComponentInParent<PlayerController>().maxHealth;
        }
        return GetComponentInParent<EnemyController>().maxLife;
    }
}
