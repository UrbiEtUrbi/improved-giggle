using UnityEngine;
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
        if (Random.value > 0.8f)
        {
            if (ControllerGame.Initialized)
            {
                SoundManager.Instance.Play("demon_growl", ControllerGame.Player.transform);
            }
        }
    }
}
