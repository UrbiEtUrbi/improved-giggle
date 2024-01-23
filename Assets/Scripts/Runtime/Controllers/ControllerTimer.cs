using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class ControllerTimer : MonoBehaviour
{

    [BeginGroup("References")]
    [SerializeField]
    SpriteRenderer Moon;

    [SerializeField]
    TMP_Text DebugTimeLabel;

    [EndGroup]
    [SerializeField]
    Light2D GlobalLight;

    [SerializeField]
    AnimationCurve MoonCurve;

    [SerializeField]
    AnimationCurve AmbientCurve;

    [SerializeField]
    [Tooltip("Game length in seconds")]
    float TimeToWin;

    float currentTime;


    float timeLeft => TimeToWin - currentTime;

    Color endColor = Color.red;
    Color startColor = Color.white;




    public UnityEvent OnTimeRanOut = new UnityEvent();

    private void Update()
    {
        if (!ControllerGame.Initialized)
        {
            return;
        }

        currentTime += Time.deltaTime;

        if (currentTime > TimeToWin)
        {
            currentTime = 0;
        }
        Moon.color = Color.Lerp(startColor, endColor, MoonCurve.Evaluate(currentTime / TimeToWin));

        DebugTimeLabel.text = $"{timeLeft:F2}";

        GlobalLight.color = Color.Lerp(startColor, endColor, AmbientCurve.Evaluate(currentTime / TimeToWin));

    }

    


}
