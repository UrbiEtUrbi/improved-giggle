public class EnemyAttackState : EnemyState {

    //::::::::::::::::::::::::::::://
    // Constructors
    //::::::::::::::::::::::::::::://

    public EnemyAttackState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine) { }
    
    //::::::::::::::::::::::::::::://
    // State Callbacks
    //::::::::::::::::::::::::::::://

    public override void EnterState() {
        enemy.AttackPlayer(() => {
            enemyStateMachine.ChangeState(enemy.IdleState);
        });    
    }
    
    public override void ExitState() { }
    public override void OnUpdate() { }
    public override void OnFixedUpdate() { }
}