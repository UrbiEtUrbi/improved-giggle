using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogSequenceTrigger : MonoBehaviour
{


    [SerializeField]
    string ID;


    private void OnTriggerEnter2D(Collider2D collision)
    {

       
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            ControllerGame.ControllerDialog.TriggerDialogue(ID, ControllerGame.Player.transform, new Vector3(0,2,0));
        }
    }
}
