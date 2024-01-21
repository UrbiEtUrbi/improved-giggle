using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ControllerRespawn : MonoBehaviour
{



    public RespawnPosition CurrentRespawn;

    public UnityEvent OnPlayerRepawned = new();


    public void Register(RespawnPosition respawnPosition)
    {

        if(CurrentRespawn == null || CurrentRespawn.transform.position.x < respawnPosition.transform.position.x){
            CurrentRespawn = respawnPosition;
        }
    }

    public void Respawn(float delay)
    {

        StartCoroutine(WaitAndRespawn(delay));
      

    }

    IEnumerator WaitAndRespawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        //move the player a bit to the right
        ControllerGame.Player.transform.position = CurrentRespawn.transform.position + new Vector3(0.1f, 0, 0);
        ControllerGame.Player.MovementController.ResetReturnPosition();
        ControllerGame.Player.IsAlive = true;
        OnPlayerRepawned.Invoke();
    }

}
