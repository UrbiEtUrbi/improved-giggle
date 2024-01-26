public class EnemyJumpState : EnemyState {
    
    //::::::::::::::::::::::::::::://
    // Constructors
    //::::::::::::::::::::::::::::://

    public EnemyJumpState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine) { }
    
    //::::::::::::::::::::::::::::://
    // State Callbacks
    //::::::::::::::::::::::::::::://

    public override void Enter() {
        enemy.SetAnimationState(Enemy.AnimationState.jump);
    }
    
    public override void Update() { }
    public override void FixedUpdate() { }
    public override void Exit() { }
}