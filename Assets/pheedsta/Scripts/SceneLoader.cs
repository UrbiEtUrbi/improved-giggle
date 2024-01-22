using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    
    //------------------------------//
    // Static Properties
    //------------------------------//

    public static SceneLoader Instance { get; private set; }
    
    //:::::::::::::::::::::::::::::://
    // Public Properties
    //:::::::::::::::::::::::::::::://

    public SceneAsset[] Scenes => scenes;
    
    //:::::::::::::::::::::::::::::://
    // Private Properties
    //:::::::::::::::::::::::::::::://

    private SceneAsset CurrentScene => scenes[currentSceneIndex];
    
    //:::::::::::::::::::::::::::::://
    // Serialized Fields
    //:::::::::::::::::::::::::::::://
    
    [Space(10)]
    [SerializeField] private SceneAsset[] scenes;
    [Space(10)]
    [SerializeField] private float proximityUnits; // rename this
    
    //:::::::::::::::::::::::::::::://
    // Enumerations
    //:::::::::::::::::::::::::::::://

    private enum LoadStatus {
        unloaded, loaded, unloading, loading
    }
    
    //:::::::::::::::::::::::::::::://
    // Readonly Fields
    //:::::::::::::::::::::::::::::://
    
    private readonly Dictionary<string, TileGrid> tileGrids = new();
    
    //:::::::::::::::::::::::::::::://
    // Local Fields
    //:::::::::::::::::::::::::::::://
    
    private int currentSceneIndex;
    private bool showScenesInEditor;
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
        // attempt to get the TileGrid for the current scene; if it's null we are done (may be null if it hasn't loaded yet)
        if (!tileGrids.TryGetValue(CurrentScene.name, out TileGrid currentTileGrid)) return;
        
        // get camera's position
        float cameraX = cameraTransform.position.x;

        if (cameraX < currentTileGrid.XMin && 0 <= currentSceneIndex - 1) {
            // camera has moved outside current scene boundary (LEFT) and new index is still in range; update scene index and unload unused scenes
            currentSceneIndex--;
            UnloadUnusedScenes();
        } else if (cameraX > currentTileGrid.XMax && currentSceneIndex + 1 < scenes.Length) {
            // camera has moved outside current scene boundary (RIGHT) and new index is still in range; update scene index and unload unused scenes
            currentSceneIndex++;
            UnloadUnusedScenes();
        }
        
        // attempt to get the TileGrid again for the current scene (current scene may have changed)
        if (!tileGrids.TryGetValue(CurrentScene.name, out currentTileGrid)) return;

        if (cameraX < currentTileGrid.XMin + proximityUnits) {
            // player is within range of next scene (LEFT); load scene
            LoadScene(currentSceneIndex - 1);
        } else if (cameraX > currentTileGrid.XMax - proximityUnits) {
            // player is within range of next scene (RIGHT); load scene
            LoadScene(currentSceneIndex + 1);
        }
    }
    
    //------------------------------//
    // Registering Tile Grids
    //------------------------------//

    public void RegisterTileGrid(TileGrid tileGrid) {
        tileGrids[tileGrid.gameObject.scene.name] = tileGrid;
    }

    public void DeregisterTileGrid(TileGrid tileGrid) {
        tileGrids.Remove(tileGrid.gameObject.scene.name);
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
        
        // start loading scene async (additive)
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scenes[sceneIndex].name, LoadSceneMode.Additive);
        
        // wait until scene has loaded
        while (!asyncOperation.isDone) yield return null;
        
        // update status to LOADED
        loadStatus[sceneIndex] = LoadStatus.loaded;
    }

    private IEnumerator UnloadSceneAdditiveCO(int sceneIndex) {
        // update status UNLOADING
        loadStatus[sceneIndex] = LoadStatus.unloading;
        
        // start unloading scene async
        AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(scenes[sceneIndex].name, UnloadSceneOptions.None);
        
        // wait until scene has unloaded
        while (!asyncOperation.isDone) yield return null;
        
        // update status to UNLOADED
        loadStatus[sceneIndex] = LoadStatus.unloaded;
    }
}
