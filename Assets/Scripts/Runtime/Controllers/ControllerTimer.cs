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

    [SerializeField]
    Color endColorFade;

    [SerializeField]
    Color endColorMoon;
    Color startColor = Color.white;




    public UnityEvent OnTimeRanOut = new UnityEvent();

    private void Update()
    {
        if (!ControllerGame.Initialized)
        {
            return;
        }

        currentTime += Time.deltaTime;

        if (currentTime > TimeToWin && ControllerGame.Player.IsAlive)
        {
            currentTime = 0;
            ControllerGame.Instance.GameOver();
            return;
        }
        Moon.color = Color.Lerp(startColor, endColorMoon, MoonCurve.Evaluate(currentTime / TimeToWin));

        DebugTimeLabel.text = $"{timeLeft:F2}";

        GlobalLight.color = Color.Lerp(startColor, endColorFade, AmbientCurve.Evaluate(currentTime / TimeToWin));

    }

    


}
