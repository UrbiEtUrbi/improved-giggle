using System;
using UnityEngine;

public abstract class Enemy : Creature {
    
    //----------------------------//
    // Properties
    //----------------------------//

    public bool IsInStrikingRange => GetIsInStrkingRange();
    
    //----------------------------//
    // Abstract Methods
    //----------------------------//

    public abstract void ChasePlayer();
    public abstract void ChasePlayerPhysics();
    public abstract void AttackPlayer(Action completionHandler);
    
    //:::::::::::::::::::::::::::://
    // Serialized Fields
    //:::::::::::::::::::::::::::://

    [Header("Enemy")]
    [SerializeField] protected float strikingDistance = 5f;
    [SerializeField] protected float moveSpeed = 10f;

    //----------------------------//
    // Enemy States
    //----------------------------//
    
    public EnemyIdleState IdleState { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }
    public EnemyAttackState AttackState { get; private set; }
    
    //:::::::::::::::::::::::::::://
    // Local Fields
    //:::::::::::::::::::::::::::://
    
    private EnemyStateMachine enemyStateMachine { get; set; }
    
    //::::::::::::::::::::::::::::://
    // Unity Callbacks
    //:::::::::::::::::::::::::::://

    protected override void Awake() {
        base.Awake(); // this will call Entity.Awake() 
        
        // construct EnemyStateMachine
        enemyStateMachine = new EnemyStateMachine();

        // construct EnemyStates
        IdleState = new EnemyIdleState(this, enemyStateMachine);
        ChaseState = new EnemyChaseState(this, enemyStateMachine);
        AttackState = new EnemyAttackState(this, enemyStateMachine);
    }

    protected override void Start() {
        base.Start(); // this will call Creature.Start()
        
        // configure state machine starting with idle state
        enemyStateMachine.Configure(IdleState);
    }

    private void Update() {
        // call state event and reset enemy direction (to face player)
        enemyStateMachine.CurrentEnemyState.OnUpdate();
    }

    private void FixedUpdate() {
        enemyStateMachine.CurrentEnemyState.OnFixedUpdate();
    }
    
    //:::::::::::::::::::::::::::://
    // Utilities
    //:::::::::::::::::::::::::::://

    private bool GetIsInStrkingRange() {
        return 0f < strikingDistance && Vector3.Distance(ControllerGame.Player.transform.position, transform.position) <= strikingDistance;
    }
}
