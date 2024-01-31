using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogSequenceInstance : MonoBehaviour
{


    Vector3 offset;
    DialogSequence sequence;
    Transform target;

    int currentSegment = 0;
    float currentSpeed = 0;
    float currentDelay = 0;
    float currentDuration = 0;
    bool currentSegmentDone = false;
    public void Init(DialogSequence _sequence, Vector3 _offset, Transform _target)
    {
        offset = _offset;
        sequence = _sequence;
        target = _target;

        currentSegmentDone = false;

        StartCoroutine(SequenceHandler());
    }

    private void Update()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    void PopuplateValues()
    {
        if (sequence.Segments[currentSegment].useCustomSettings)
        {
            currentSpeed = sequence.Segments[currentSegment].CustomSpeed;
            currentDuration = sequence.Segments[currentSegment].CustomDuration;
            currentDelay = sequence.Segments[currentSegment].CustomDelay;
        }
        else
        {
            currentSpeed = sequence.DefaultSpeed;
            currentDuration = sequence.DefaultDuration;
            currentDelay = sequence.DefaultDelay;
        }

    }



    IEnumerator SequenceHandler()
    {
        while (true)
        {
            
            while (true)
            {


            }
            yield return new WaitUntil(() => currentSegmentDone);
            currentSegment++;

        }

    }


}
