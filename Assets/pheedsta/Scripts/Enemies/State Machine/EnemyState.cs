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
    
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
}
