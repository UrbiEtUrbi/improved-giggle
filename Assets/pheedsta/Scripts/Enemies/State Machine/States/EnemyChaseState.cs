public class EnemyChaseState : EnemyState {
    
    //::::::::::::::::::::::::::::://
    // Constructors
    //::::::::::::::::::::::::::::://

    public EnemyChaseState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine) { }
    
    //::::::::::::::::::::::::::::://
    // State Callbacks
    //::::::::::::::::::::::::::::://

    public override void Enter() {
        // update chase state (this will make enemy move towards player)
        enemy.IsChasingPlayer = true;
        
        // update animation state
        enemy.SetAnimationState(Enemy.AnimationState.chase);
    }

    public override void FixedUpdate() {
        if (!enemy.IsActive) {
            // enemy is no longer in range of player; move to idle state
            enemyStateMachine.ChangeState(enemy.IdleState);
        } else if (enemy.WillFall) {
            // enemy will fall if it continues to follow player; move to idle state
            enemyStateMachine.ChangeState(enemy.IdleState);
        } else if (enemy.IsInStrikingRange && enemy.IsGrounded) {
            // enemy is within striking distance AND is grounded; move to attack state
            enemyStateMachine.ChangeState(enemy.AttackState);
        } else if (enemy.ShouldJump) {
            // enemy is close to a wall; move to jump state
            enemyStateMachine.ChangeState(enemy.JumpState);
        }
    }
    
    //::::::::::::::::::::::::::::://
    // State Callbacks (unused)
    //::::::::::::::::::::::::::::://
    
    public override void Update() { }
    public override void ExecuteAttack() { }
    public override void Exit() { }
}
