using System;
using System.Collections;
using UnityEngine;

public class EnemyGoombaSub : EnemyGoomba {

    //----------------------------//
    // Enemy Abstract Methods
    //----------------------------//
    
    public override void Attack(Action completionHandler) {
        base.Attack(completionHandler);

        // start coroutine (this will call the completionHandler when finished)
        StartCoroutine(AttackPlayerCO(completionHandler));
        
        // instantiate attack object
        ControllerGame.ControllerAttack.Attack(transform, true, AttackType.GoombaAttack,
            transform.position + new Vector3((IsFacingRight ? 1 : -1) * strikingDistance, 0, 0),
            strikingSize, 1, 0.5f);
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
}
