public class EnemyJumpState : EnemyState {
    
    //::::::::::::::::::::::::::::://
    // Constructors
    //::::::::::::::::::::::::::::://

    public EnemyJumpState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine) { }
    
    //::::::::::::::::::::::::::::://
    // State Callbacks
    //::::::::::::::::::::::::::::://

    public override void Enter() {
        // update chase state (this will make enemy move towards player in mid air)
        enemy.IsChasingPlayer = true;
        
        // jump towards player; when finished return to idle state
        enemy.Jump(() => {
            enemyStateMachine.ChangeState(enemy.IdleState);
        });
        
        // update animation state
        enemy.SetAnimationState(Enemy.AnimationState.jump);
    }
    
    public override void Update() { }
    public override void FixedUpdate() { }
    public override void Exit() { }
}