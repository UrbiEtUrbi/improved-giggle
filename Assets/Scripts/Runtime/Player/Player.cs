using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IHealth
{



    [BeginGroup("Health")]
    [EndGroup]
    [SerializeField]
    int MaxHealth;

    [BeginGroup("Attack")]
    [SerializeField]
    int Damage;

    [SerializeField]
    Vector3 AttackSize, AttackPosition;
    [EndGroup]
    [EditorButton(nameof(Die))]
    [SerializeField]
    float ReloadTime;

   
    int currentHealth;

    float reloadTimer;

    [HideInInspector]
    public PlayerMovementController MovementController;

    public bool IsAlive;


    private void Awake()
    {
        MovementController = GetComponent<PlayerMovementController>();
    }




    public void Attack()
    {
        if (reloadTimer > 0)
        {
            return;
        }

        ControllerGame.ControllerAttack.Attack(AttackType.PlayerSword, transform.position + (MovementController.FacingRight ? 1 : -1 )* AttackPosition, AttackSize, Damage, 0.2f);
        reloadTimer = ReloadTime;
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, MaxHealth);

        if (amount < 0)
        {
            //spawn damage vfx
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Update()
    {
        reloadTimer -= Time.deltaTime;
    }

    public void Die()
    {
        //spawn death vfx/animation
        IsAlive = false;
        ControllerGame.Instance.PlayerDie();
    }

    public void SetInitialHealth(int amount)
    {
        currentHealth = MaxHealth = amount;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position+AttackPosition, AttackSize);
    }
}
