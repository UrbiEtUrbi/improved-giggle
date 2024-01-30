public class EnemyAttackState : EnemyState {

    //::::::::::::::::::::::::::::://
    // Constructors
    //::::::::::::::::::::::::::::://

    public EnemyAttackState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine) { }
    
    //::::::::::::::::::::::::::::://
    // State Callbacks
    //::::::::::::::::::::::::::::://

    public override void Enter() {
        // update chase state (this will stop enemy moving towards player)
        enemy.IsChasingPlayer = false;
        
        // start attacking player; when finished return to idle state
        enemy.Attack(() => {
            enemyStateMachine.ChangeState(enemy.IdleState);
        });
        
        // update animation state
        enemy.SetAnimationState(Enemy.AnimationState.attack);
    }
    
    //::::::::::::::::::::::::::::://
    // State Callbacks (unused)
    //::::::::::::::::::::::::::::://
    
    public override void Update() { }
    public override void FixedUpdate() { }
    public override void ExecuteAttack() { }
    public override void Exit() { }
}