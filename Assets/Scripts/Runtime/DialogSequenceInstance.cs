using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

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

    [SerializeField]
    TMP_Text Text;

    UnityAction<Transform> Action;

    string currentText;
    public void Init(DialogSequence _sequence, Vector3 _offset, Transform _target, UnityAction<Transform> action)
    {

        transform.position = _target.position + _offset;
        offset = _offset;
        sequence = _sequence;
        target = _target;
        Action = action;

        currentSegmentDone = false;

        PopuplateValues();
        StartCoroutine(SequenceHandler());

    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            transform.position = Vector3.Lerp(transform.position, target.position + offset, 0.2f);
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
        int currentCharacter = 0;
        while (true)
        {
            yield return new WaitForSeconds(currentDelay);
            while (true)
            {

                if (sequence.Segments[currentSegment].Text[currentCharacter] == '<'){

                    while (sequence.Segments[currentSegment].Text[currentCharacter] != '>')
                    {
                        currentCharacter++;
                    }
                    currentCharacter++;
                }
                currentText = sequence.Segments[currentSegment].Text.Substring(0, currentCharacter);
                Text.text = currentText;
                yield return new WaitForSeconds(currentSpeed);
                currentCharacter++;
                if (currentCharacter >= sequence.Segments[currentSegment].Text.Length){
                    break;
                }
                
            }

            float time = 0;

            while (time < currentDuration)
            {
                yield return new WaitForEndOfFrame();
                time += Time.deltaTime;
                Text.alpha = 1 - ((time*time) / (currentDuration*currentDuration));
            }


           
            currentSegment++;
            currentCharacter = 0;
            if (currentSegment >= sequence.Segments.Count)
            {
                Action.Invoke(target);
                Destroy(gameObject);
                break;
            }
            PopuplateValues();
            Text.text = string.Empty;
            Text.alpha = 1;
        }


    }


}
