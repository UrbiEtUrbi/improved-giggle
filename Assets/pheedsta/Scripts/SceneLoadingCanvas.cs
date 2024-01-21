using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SceneLoadingCanvas : MonoBehaviour {
    private TextMeshProUGUI sceneTextTMP;
    private TextMeshProUGUI loadingTextTMP;

    private void Awake() {
        sceneTextTMP = transform.Find("Scene Text").GetComponent<TextMeshProUGUI>();
        loadingTextTMP = transform.Find("Loading Text").GetComponent<TextMeshProUGUI>();
    }

    private void Update() {

        if (!ControllerGame.Initialized)
        {
            return;
        }
        sceneTextTMP.SetText($"Scene: {ControllerGame.SceneLoader.CurrentSceneIndex}");
        loadingTextTMP.SetText(ControllerGame.SceneLoader.LoadingDescription);
    }
}
