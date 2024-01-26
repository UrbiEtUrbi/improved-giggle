public class EnemyChaseState : EnemyState {
    
    //::::::::::::::::::::::::::::://
    // Constructors
    //::::::::::::::::::::::::::::://

    public EnemyChaseState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine) { }
    
    //::::::::::::::::::::::::::::://
    // State Callbacks
    //::::::::::::::::::::::::::::://

    public override void Enter() {
        // start chasing the player
        enemy.IsChasingPlayer = true;
        
        // update animation state
        enemy.SetAnimationState(Enemy.AnimationState.chase);
    } 
    
    public override void Update() { } 

    public override void FixedUpdate() {
        if (!enemy.IsActive) {
            // enemy is no longer in range of player; move to idle state
            enemyStateMachine.ChangeState(enemy.IdleState);
        } else if (enemy.IsInStrikingRange && enemy.IsGrounded) {
            // enemy is within striking distance AND is grounded; move to attack state
            enemyStateMachine.ChangeState(enemy.AttackState);
        }
    }

    public override void Exit() {
        // stop chasing the player
        enemy.IsChasingPlayer = false;
    }
}
