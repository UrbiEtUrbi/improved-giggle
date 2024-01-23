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
        CurrentEnemyState.EnterState();
    }
    
    //-----------------------------//
    // Transitioning Between States
    //-----------------------------//

    public void ChangeState(EnemyState newState) {
        CurrentEnemyState.ExitState();
        CurrentEnemyState = newState;
        CurrentEnemyState.EnterState();
    }
}
