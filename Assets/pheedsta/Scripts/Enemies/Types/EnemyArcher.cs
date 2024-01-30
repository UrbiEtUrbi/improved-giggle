using System;
using System.Collections;
using UnityEngine;

public class EnemyArcher : EnemyGoomba {

    //----------------------------//
    // Enemy Abstract Methods
    //----------------------------//
    
    public override void Attack(Action completionHandler) {
        base.Attack(completionHandler);

        // start coroutine (this will call the completionHandler when finished)
        StartCoroutine(AttackPlayerCO(completionHandler));
        
        // instantiate attack object
        ControllerGame.ControllerAttack.Attack(transform, false, AttackType.ArcherAttack, transform.position, strikingSize, 1, 5f);
    }

    //::::::::::::::::::::::::::::://
    // Coroutines
    //::::::::::::::::::::::::::::://

    private IEnumerator AttackPlayerCO(Action completionHandler) {
        // for now just attack for 2 seconds
        yield return new WaitForSeconds(2f);
        
        // we have finished attack; invoke callback
        completionHandler?.Invoke();
    }
    
}
