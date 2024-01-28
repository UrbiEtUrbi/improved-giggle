using System;
using System.Collections;
using UnityEngine;

public class EnemyGoomba : Enemy {
    
    //:::::::::::::::::::::::::::://
    // Constants
    //:::::::::::::::::::::::::::://
    
    private const float movementThreshold = 0.5f;
    
    //::::::::::::::::::::::::::::://
    // Unity Callbacks
    //::::::::::::::::::::::::::::://

    protected override void FixedUpdate() {
        base.FixedUpdate();

        if (IsChasingPlayer) {
            Accelerate();
        } else {
            Decelerate();
        }
    }

    //----------------------------//
    // Enemy Abstract Methods
    //----------------------------//

    public override void Attack(Action completionHandler) {


        ControllerGame.ControllerAttack.Attack(transform, true, AttackType.GoombaAttack,
            transform.position + new Vector3((IsFacingRight ? 1 : -1) * strikingDistance, 0, 0),
            strikingSize, 1, 0.5f);
        StartCoroutine(AttackPlayerCO(completionHandler));
    }

    public override void Jump(Action completionHandler) {
        StartCoroutine(JumpCO(completionHandler));
    }

    //::::::::::::::::::::::::::::://
    // Movement
    //::::::::::::::::::::::::::::://
    
    private void Accelerate() {
        // calculate the difference between the player and enemy (x)
        float xDifference = ControllerGame.Player.transform.position.x - transform.position.x;
        
        if (Mathf.Abs(xDifference) < movementThreshold) {
            // movement is less than threshold; decelerate instead
            // this prevents enemy from constantly moving back and forth when under player
            Decelerate();
        } else {
            // movement is greater than threshold; get move direction (left = -1, right = 1)
            float moveDirection = 0f < xDifference ? 1f : -1f;

            // calculate force required
            float targetSpeed = moveSpeed * moveDirection;
            float speedDifference = targetSpeed - Rigidbody.velocity.x;

            // apply force
            Rigidbody.AddForce(Vector2.right * (speedDifference * acceleration), ForceMode2D.Force);
        }
    }

    private void Decelerate() {
        float speedDifference = 0f - Rigidbody.velocity.x;
        Rigidbody.AddForce(Vector2.right * (speedDifference * deceleration), ForceMode2D.Force);
    }

    //::::::::::::::::::::::::::::://
    // Coroutines
    //::::::::::::::::::::::::::::://

    private IEnumerator AttackPlayerCO(Action completionHandler) {
        // for now just attack for half a second
        yield return new WaitForSeconds(0.5f);
        
        // we have finished attack; invoke callback
        completionHandler?.Invoke();
    }

    private IEnumerator JumpCO(Action completionHandler) {
        // add jump force to enemy (this will launch enemy into the air)
        Rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        
        // wait until next fixed update so it's feet have definitely left the ground
        yield return new WaitForFixedUpdate();

        // wait until enemy it is grounded again
        while (!IsGrounded) yield return null;
        
        // we have finished jump; invoke callback
        completionHandler?.Invoke();
    }
}
