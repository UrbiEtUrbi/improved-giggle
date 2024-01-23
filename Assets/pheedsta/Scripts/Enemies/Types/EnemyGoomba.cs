using System;
using System.Collections;
using UnityEngine;

public class EnemyGoomba : Enemy {
    
    private bool isFacingLeft = true;
    
    //::::::::::::::::::::::::::::://
    // Unity Callbacks
    //::::::::::::::::::::::::::::://

    private void LateUpdate() {
        //----------------------------//
        // THIS IS FOR DEMONSTRATION PURPOSES ONLY!!!
        //----------------------------//

        Vector3 playerPosition = ControllerGame.Player.transform.position;
        
        if ((isFacingLeft && transform.position.x > playerPosition.x) || (!isFacingLeft && transform.position.x < playerPosition.x)) return;
        
        transform.Rotate(Vector3.up, 180f);
        isFacingLeft = !isFacingLeft;
    }

    //----------------------------//
    // Enemy Abstract Methods
    //----------------------------//
    
    public override void ChasePlayer() {
        //----------------------------//
        // THIS IS FOR DEMONSTRATION PURPOSES ONLY!!!
        //----------------------------//
        Vector3 currentPosition = transform.position;
        Vector3 playerPosition = new(ControllerGame.Player.transform.position.x, currentPosition.y, currentPosition.z);
        transform.position = Vector3.MoveTowards(currentPosition, playerPosition, moveSpeed * Time.deltaTime);
    }

    // we aren't using physics for this enemy
    public override void ChasePlayerPhysics() { }

    public override void AttackPlayer() {
        StartCoroutine(AttackPlayerCO());
    }
    
    //::::::::::::::::::::::::::::://
    // Coroutines
    //::::::::::::::::::::::::::::://

    private IEnumerator AttackPlayerCO() {
        //----------------------------//
        // THIS IS FOR DEMONSTRATION PURPOSES ONLY!!!
        //----------------------------//
        
        const float jumpDuration = 0.15f;
        const float jumpHeight = 0.6f;
        
        float progress = 0f;
        Vector3 startPosition = transform.position;
        Vector3 newPosition = startPosition;

        while (progress < jumpDuration) {
            newPosition.y = startPosition.y + progress / jumpDuration * jumpHeight;
            transform.position = newPosition;
            yield return null;
            progress += Time.deltaTime;
        }

        while (0f < progress) {
            newPosition.y = startPosition.y + progress / jumpDuration * jumpHeight;
            transform.position = newPosition;
            yield return null;
            progress -= Time.deltaTime;
        }
        
        transform.position = startPosition;
        enemyStateMachine.ChangeState(IdleState);
    }
}
