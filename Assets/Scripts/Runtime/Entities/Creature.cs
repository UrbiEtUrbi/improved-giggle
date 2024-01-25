using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : Entity, IHealth
{

    [BeginGroup("Health")]
    [EndGroup]
    [SerializeField]
    int MaxHealth;


    int currentHealth;




    protected virtual void Start()
    {
        currentHealth = MaxHealth;
    }


    public void ChangeHealth(int amount)
    {
        Debug.Log(amount);
        Debug.Log(currentHealth);
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, MaxHealth);

        Debug.Log(currentHealth);
        if (amount < 0)
        {
            //spawn damage vfx
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        ControllerGame.ControllerAttack.OnEnemyDied();
        //spawn death animation
        Destroy(gameObject);
    }

    public void SetInitialHealth(int amount) {
        currentHealth = amount;
    }
}
