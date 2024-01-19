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
        sceneTextTMP.SetText($"Scene: {SceneLoader.Instance.CurrentSceneIndex}");
        loadingTextTMP.SetText(SceneLoader.Instance.LoadingDescription);
    }
}
