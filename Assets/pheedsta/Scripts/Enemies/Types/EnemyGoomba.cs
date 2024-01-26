using System;
using System.Collections;
using UnityEngine;

public class EnemyGoomba : Enemy {
    
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
        StartCoroutine(AttackPlayerCO(completionHandler));
    }
    
    private void Accelerate() {
        float moveDirection = 0f < ControllerGame.Player.transform.position.x - transform.position.x ? 1f : -1f;
        float targetSpeed = moveSpeed * moveDirection;
        float speedDiff = targetSpeed - Rigidbody.velocity.x;
        Move(speedDiff * acceleration);
    }

    private void Decelerate() {
        float speedDiff = 0f - Rigidbody.velocity.x;
        Move(speedDiff * deceleration);
    }

    private void Move(float m) {
        Rigidbody.AddForce(Vector2.right * m, ForceMode2D.Force);
    }

    //::::::::::::::::::::::::::::://
    // Coroutines
    //::::::::::::::::::::::::::::://

    private IEnumerator AttackPlayerCO(Action completionHandler) {
        // for now just attack for half second
        yield return new WaitForSeconds(0.5f);
        completionHandler?.Invoke();
    }
}
