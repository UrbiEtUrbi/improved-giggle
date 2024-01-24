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

    public void Respawn(float delay, Vector3 pos = default)
    {

        if (pos == default)
        {

            pos = CurrentRespawn.transform.position;
        }
        StartCoroutine(WaitAndRespawn(delay, pos));
      

    }

    IEnumerator WaitAndRespawn(float delay, Vector3 pos)
    {
        yield return new WaitForSeconds(delay);
        //move the player a bit to the right
        ControllerGame.Player.transform.position = pos + new Vector3(0.1f, 0, 0);
        ControllerGame.Player.MovementController.ResetReturnPosition();
        ControllerGame.Player.IsAlive = true;
        OnPlayerRepawned.Invoke();
    }

}
