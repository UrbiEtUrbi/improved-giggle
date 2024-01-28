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




    AttackObject attackObject;

    public bool Attack()
    {
        if (reloadTimer > 0)
        {
            return false;
        }
        attackObject = ControllerGame.ControllerAttack.Attack(transform, true,AttackType.PlayerSword, transform.position + (MovementController.FacingRight ? 1 : -1 )* AttackPosition, AttackSize, Damage, 0.2f);
        reloadTimer = ReloadTime;
        return true;
    }

    public void OnAttackEnd()
    {
        if (attackObject != null)
        {
            Destroy(attackObject.gameObject);
        }
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

    public void OnEnemyDied()
    {
        MovementController.ChangeEnemyCount(1);
    }

    public void SetInitialHealth(int amount)
    {
        currentHealth = MaxHealth = amount;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        var attackpos = new Vector3((MovementController.FacingRight ? 1 : -1) * AttackPosition.x, AttackPosition.y);
        Gizmos.DrawWireCube(transform.position+ attackpos, AttackSize);
    }
}
