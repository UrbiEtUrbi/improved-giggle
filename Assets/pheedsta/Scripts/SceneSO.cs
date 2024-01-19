using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scene", menuName = "Scriptable Objects/Scene", order = 0)]
public class SceneSO : ScriptableObject {

    public string SceneName => sceneName;
    public float BoundaryMinX => boundaryMinX;
    public float BoundaryMaxX => boundaryMaxX;
    
    
    [SerializeField] private string sceneName;
    [SerializeField] private float boundaryMinX;
    [SerializeField] private float boundaryMaxX;
}
