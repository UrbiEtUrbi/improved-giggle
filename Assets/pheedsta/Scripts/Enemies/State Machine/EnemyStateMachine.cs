public class EnemyStateMachine {
    
    //-----------------------------//
    // Properties
    //-----------------------------//
    
    public EnemyState CurrentEnemyState { get; private set; }
    
    //-----------------------------//
    // Configuration
    //-----------------------------//

    public void Configure(EnemyState startingState) {
        CurrentEnemyState = startingState;
        CurrentEnemyState.Enter();
    }
    
    //-----------------------------//
    // Transitioning Between States
    //-----------------------------//

    public void ChangeState(EnemyState newState) {
        CurrentEnemyState.Exit();
        CurrentEnemyState = newState;
        CurrentEnemyState.Enter();
    }
}
