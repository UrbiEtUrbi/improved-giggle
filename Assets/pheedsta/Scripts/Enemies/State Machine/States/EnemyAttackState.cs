public class EnemyAttackState : EnemyState {

    //::::::::::::::::::::::::::::://
    // Constructors
    //::::::::::::::::::::::::::::://

    public EnemyAttackState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine) { }
    
    //::::::::::::::::::::::::::::://
    // State Callbacks
    //::::::::::::::::::::::::::::://

    public override void Enter() {
        // start attacking player; when finished return to idle state
        enemy.Attack(() => {
            enemyStateMachine.ChangeState(enemy.IdleState);
        });
        
        // update animation state
        enemy.SetAnimationState(Enemy.AnimationState.attack);
    }
    
    public override void Update() { }
    public override void FixedUpdate() { }
    public override void Exit() { }
}