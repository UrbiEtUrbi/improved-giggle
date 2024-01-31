using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerDialog : MonoBehaviour
{

    public List<DialogSequence> Sequences;


    public List<string> Triggered;


    public void TriggerDialogue(string ID, Transform Target, Vector3 offset = default)
    {


    }






}



public class DialogSequence{

    public string ID;
    public float DefaultDelay;
    public float DefaultDuration;
    public float DefaultSpeed;
    public bool Repeatable;
}

public class DialogSegment
{
    public float CustomDelay = -1;
    public float DefaultDuration = -1;
    public float DefaultSpeed = -1;
    public string Text;

}