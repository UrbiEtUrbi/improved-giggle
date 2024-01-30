public abstract class EnemyState {
    
    //::::::::::::::::::::::::::::://
    // Protected Fields
    //::::::::::::::::::::::::::::://
    
    protected readonly Enemy enemy;
    protected readonly EnemyStateMachine enemyStateMachine;
    
    //::::::::::::::::::::::::::::://
    // Constructors
    //::::::::::::::::::::::::::::://

    protected EnemyState(Enemy enemy, EnemyStateMachine enemyStateMachine) {
        this.enemy = enemy;
        this.enemyStateMachine = enemyStateMachine;
    }
    
    //-----------------------------//
    // Virtual Methods
    //-----------------------------//

    public abstract void Enter();
    public abstract void Update();
    public abstract void FixedUpdate();
    public abstract void ExecuteAttack();
    public abstract void Exit();
}
