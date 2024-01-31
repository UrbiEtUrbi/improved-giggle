using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerDialog : MonoBehaviour
{

    [ReorderableList]
    public List<DialogSequence> Sequences;


    [HideInInspector]
    
    public List<string> Triggered;


    public void TriggerDialogue(string ID, Transform Target, Vector3 offset = default)
    {


    }






}



[System.Serializable]
public class DialogSequence{

    public string ID;
    public float DefaultDelay = 1;
    public float DefaultDuration = 1;
    public int DefaultSpeed = 10;
    public bool Repeatable;

    [ReorderableList]
    public List<DialogSegment> Segments;
}


[System.Serializable]
public class DialogSegment
{
    [ShowIf(nameof(useCustomSettings),true)]
    public float CustomDelay = -1;
    [ShowIf(nameof(useCustomSettings), true)]
    public float CustomDuration = -1;
    [ShowIf(nameof(useCustomSettings), true)]
    public int CustomSpeed = -1;
    public bool useCustomSettings;
    public string Text;

}