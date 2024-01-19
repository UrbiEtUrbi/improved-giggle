using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    
    //------------------------------//
    // Static Properties
    //------------------------------//

    public static SceneLoader Instance { get; private set; }
    
    //:::::::::::::::::::::::::::::://
    // Serialized Fields
    //:::::::::::::::::::::::::::::://

    [SerializeField] private SceneSO[] scenes;
    [SerializeField] private float proximityUnits; // rename this
    
    //:::::::::::::::::::::::::::::://
    // Properties
    //:::::::::::::::::::::::::::::://

    private SceneSO CurrentSceneSO => scenes[currentSceneIndex];
    
    //:::::::::::::::::::::::::::::://
    // Enumerations
    //:::::::::::::::::::::::::::::://

    private enum LoadStatus {
        unloaded, loaded, unloading, loading
    }
    
    //:::::::::::::::::::::::::::::://
    // Local Fields
    //:::::::::::::::::::::::::::::://

    private int currentSceneIndex;
    private Transform cameraTransform;
    private LoadStatus[] loadStatus;
    
    //:::::::::::::::::::::::::::::://
    // Unity Callbacks
    //:::::::::::::::::::::::::::::://

    private void Awake() {
        if (Instance) {
            // an instance has already been set; just destroy this GameObject
            Destroy(gameObject);
        } else {
            // an instance has not been set; update static field
            Instance = this;
            
            // get main camera transform (we use this to determine when to load new scenes)
            cameraTransform = Camera.main.transform;
        }
    }

    private void Start() {
        // initialise load status array (same length as scenes array) and prepopulate with default values (UNLOADED)
        loadStatus = new LoadStatus[scenes.Length];
        Array.Fill(loadStatus, LoadStatus.unloaded);
        
        // start loading the first scene
        LoadScene(currentSceneIndex);
    }

    private void Update() {
        // get camera's position
        float cameraX = cameraTransform.position.x;

        if (cameraX < CurrentSceneSO.BoundaryMinX && 0 <= currentSceneIndex - 1) {
            // camera has moved outside current scene boundary (LEFT) and new index is still in range; update scene index and unload unused scenes
            currentSceneIndex--;
            UnloadUnusedScenes();
            
            // DELETE ME
            AddLoadingDescription($"Updating active scene: {currentSceneIndex}");
                
        } else if (cameraX > CurrentSceneSO.BoundaryMaxX && currentSceneIndex + 1 < scenes.Length) {
            // camera has moved outside current scene boundary (RIGHT) and new index is still in range; update scene index and unload unused scenes
            currentSceneIndex++;
            UnloadUnusedScenes();
            
            // DELETE ME
            AddLoadingDescription($"Updating active scene: {currentSceneIndex}");
        }

        if (cameraX < CurrentSceneSO.BoundaryMinX + proximityUnits) {
            // player is within range of next scene (LEFT); load scene
            LoadScene(currentSceneIndex - 1);
        } else if (cameraX > CurrentSceneSO.BoundaryMaxX - proximityUnits) {
            // player is within range of next scene (RIGHT); load scene
            LoadScene(currentSceneIndex + 1);
        }
    }
    
    //:::::::::::::::::::::::::::::://
    // Loading & Unloading Scenes
    //:::::::::::::::::::::::::::::://

    private void LoadScene(int sceneIndex) {
        // if scene index is out of range; ignore call
        if (sceneIndex < 0 || sceneIndex >= scenes.Length) return;
        
        // if scene is not unloaded; ignore call
        if (loadStatus[sceneIndex] != LoadStatus.unloaded) return;
        
        // load scene
        StartCoroutine(LoadSceneAdditiveCO(sceneIndex));
    }

    private void UnloadUnusedScenes() {
        for (int i = 0; i < scenes.Length; i++) {
            // this scene is not loaded; ignore it
            if (loadStatus[i] != LoadStatus.loaded) continue;
            
            // if this scene is no longer in range; start coroutine to unload it
            if (i < currentSceneIndex - 1 || i > currentSceneIndex + 1) StartCoroutine(UnloadSceneAdditiveCO(i));
        }
    }
    
    //:::::::::::::::::::::::::::::://
    // Coroutines
    //:::::::::::::::::::::::::::::://

    private IEnumerator LoadSceneAdditiveCO(int sceneIndex) {
        // update status to LOADING
        loadStatus[sceneIndex] = LoadStatus.loading;
        
        // DELETE ME
        AddLoadingDescription($"Start loading scene: {sceneIndex}");
        
        // start loading scene async (additive)
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scenes[sceneIndex].SceneName, LoadSceneMode.Additive);
        
        // wait until scene has loaded
        while (!asyncOperation.isDone) yield return null;
        
        // update status to LOADED
        loadStatus[sceneIndex] = LoadStatus.loaded;
        
        // DELETE ME
        AddLoadingDescription($"Completed loading scene: {sceneIndex}");
    }

    private IEnumerator UnloadSceneAdditiveCO(int sceneIndex) {
        // update status UNLOADING
        loadStatus[sceneIndex] = LoadStatus.unloading;
        
        // DELETE ME
        AddLoadingDescription($"Start unloading scene: {sceneIndex}");
        
        // start unloading scene async
        AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(scenes[sceneIndex].SceneName, UnloadSceneOptions.None);
        
        // wait until scene has unloaded
        while (!asyncOperation.isDone) yield return null;
        
        // update status to UNLOADED
        loadStatus[sceneIndex] = LoadStatus.unloaded;
        
        // DELETE ME
        AddLoadingDescription($"Completed unloading scene: {sceneIndex}");
    }
    
    
    
    
    
    
    
    
    
    
    
    //------------------------------//
    // Properties
    //------------------------------//

    public int CurrentSceneIndex => currentSceneIndex;
    public string LoadingDescription => loadingDescription;
    private string loadingDescription;

    private void AddLoadingDescription(string s) {
        loadingDescription += $"{s}\n";
    }
}
