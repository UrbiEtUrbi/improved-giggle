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

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void OnUpdate() { }
    public virtual void OnFixedUpdate() { }
}
