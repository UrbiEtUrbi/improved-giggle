public class EnemyIdleState : EnemyState {

    //::::::::::::::::::::::::::::://
    // Constructors
    //::::::::::::::::::::::::::::://
    
    public EnemyIdleState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine) { }
    
    //::::::::::::::::::::::::::::://
    // State Callbacks
    //::::::::::::::::::::::::::::://

    public override void Enter() {
        // update chase state (this will stop enemy moving towards player)
        enemy.IsChasingPlayer = false;
        
        // update animation state
        enemy.SetAnimationState(Enemy.AnimationState.idle);
    }
    
    public override void Update() {
        // when enemy becomes active AND it is grounded AND it is not going to fall; change to chase state
        if (enemy.IsActive && enemy.IsGrounded && !enemy.WillFall) enemyStateMachine.ChangeState(enemy.ChaseState);
    }
    
    public override void FixedUpdate() { }
    public override void Exit() { }
}