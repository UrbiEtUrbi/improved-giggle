public class EnemyChaseState : EnemyState {
    
    //::::::::::::::::::::::::::::://
    // Constructors
    //::::::::::::::::::::::::::::://

    public EnemyChaseState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine) { }
    
    //::::::::::::::::::::::::::::://
    // State Callbacks
    //::::::::::::::::::::::::::::://

    public override void OnUpdate() {
        if (!enemy.IsActive) {
            // enemy is no longer active; change to idle state
            enemyStateMachine.ChangeState(enemy.IdleState);
        } else if (enemy.IsInStrikingRange) {
            // enemy is within striking range of player; change to attack state
            enemyStateMachine.ChangeState(enemy.AttackState);
        } else {
            // move towards player
            enemy.ChasePlayer(); 
        }
    }

    public override void OnFixedUpdate() {
        // move towards player (using physics)
        enemy.ChasePlayerPhysics();
    }

    public override void EnterState() { }
    public override void ExitState() { }
}
