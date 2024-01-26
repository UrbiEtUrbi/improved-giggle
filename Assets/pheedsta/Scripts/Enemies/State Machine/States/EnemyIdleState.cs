using UnityEngine;

public class EnemyIdleState : EnemyState {

    //::::::::::::::::::::::::::::://
    // Constructors
    //::::::::::::::::::::::::::::://
    
    public EnemyIdleState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine) { }
    
    //::::::::::::::::::::::::::::://
    // State Callbacks
    //::::::::::::::::::::::::::::://

    public override void Enter() {
        enemy.SetAnimationState(Enemy.AnimationState.idle);
    }
    
    public override void Update() {
        // when enemy becomes active AND it is grounded; change to chase state
        if (enemy.IsActive && enemy.IsGrounded) enemyStateMachine.ChangeState(enemy.ChaseState);
    }
    
    public override void FixedUpdate() { }
    public override void Exit() { }
}