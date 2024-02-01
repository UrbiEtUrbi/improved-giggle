using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPosition : MonoBehaviour
{

    public void Register()
    {
        ControllerGame.ControllerRespawn.Register(this);
    }


    private void FixedUpdate()
    {

        if (ControllerGame.Player == null)
        {
            return;
        }
        if (ControllerGame.Player.transform.position.x > transform.position.x)
        {
            Register();
            gameObject.SetActive(false);
        }
    }
}
