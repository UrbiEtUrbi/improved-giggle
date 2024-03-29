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
    [EditorButton(nameof(TestDie))]
    [SerializeField]
    float ReloadTime;


    [EndGroup]
    [SerializeField]
    float InvulTime = 1f;
   
    int currentHealth;

    float reloadTimer;
    float invulTimer;

    [SerializeField]
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

        SoundManager.Instance.Play("tomoe_attack", transform);
        attackObject = ControllerGame.ControllerAttack.Attack(transform, true,AttackType.PlayerSword, transform.position + (MovementController.FacingRight ? 1 : -1 )* AttackPosition, AttackSize, Damage, 2);
        reloadTimer = ReloadTime;
        invulTimer = InvulTime;
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

        if (amount < 0 && invulTimer > 0 || ControllerGame.Instance.IsGameOver)
        {
            //invincible
            return;
        }
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, MaxHealth);

        if (amount < 0)
        {
            //spawn damage vfx
        }
        if (currentHealth <= 0 && IsAlive)
        {
            MovementController.Die();
            Die();
        }
    }

    public void TestDie()
    {
        MovementController.Die();
        Die();
    }

    private void Update()
    {
        reloadTimer -= Time.deltaTime;
        invulTimer -= Time.deltaTime;

        if (transform.position.y < -20f && IsAlive)
        {
            TestDie();
        }
    }

    public void Die()
    {


        SoundManager.Instance.Play("tomoe_dead", transform);
        IsAlive = false;
        ControllerGame.Instance.PlayerDie();
    }

    public void RespawnInvul(float time)
    {
        invulTimer = time;
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
