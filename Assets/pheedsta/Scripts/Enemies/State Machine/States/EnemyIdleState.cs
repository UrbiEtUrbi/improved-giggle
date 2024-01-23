public class EnemyIdleState : EnemyState {

    //::::::::::::::::::::::::::::://
    // Constructors
    //::::::::::::::::::::::::::::://
    
    public EnemyIdleState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine) { }
    
    //::::::::::::::::::::::::::::://
    // State Callbacks
    //::::::::::::::::::::::::::::://

    public override void OnUpdate() {
        // if enemy is active; change to chase state
        if (enemy.IsActive) enemyStateMachine.ChangeState(enemy.ChaseState);
    }

    public override void EnterState() { }
    public override void ExitState() { }
    public override void OnFixedUpdate() { }
}