using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class SceneLoader : MonoBehaviour {
    
    //------------------------------//
    // Static Properties
    //------------------------------//

    public static SceneLoader Instance { get; private set; }

    //:::::::::::::::::::::::::::::://
    // Public Properties
    //:::::::::::::::::::::::::::::://
    [HideInInspector]
    [SerializeField]
    public string[] SceneNames;

#if UNITY_EDITOR
    public SceneAsset[] Scenes => scenes;
    
    [Space(10)]
    [SerializeField] private SceneAsset[] scenes;
    //:::::::::::::::::::::::::::::://
    // Private Properties
    //:::::::::::::::::::::::::::::://


    private void OnValidate()
    {
        if (scenes != null)
        {
            SceneNames = new string[scenes.Length];
           for(int i = 0; i < SceneNames.Length;i++)
            {
                SceneNames[i] = scenes[i].name;
            }

        }
    }
#endif

    private string CurrentScene => SceneNames[currentSceneIndex];

    //:::::::::::::::::::::::::::::://
    // Serialized Fields
    //:::::::::::::::::::::::::::::://

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
    private bool registeredTilemapEvents;
    private Transform cameraTransform;
    private LoadStatus[] loadStatus;

    public bool FirstSceneLoaded = false;
    
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
        loadStatus = new LoadStatus[SceneNames.Length];
        Array.Fill(loadStatus, LoadStatus.unloaded);
        
        // start loading the first scene
        LoadScene(currentSceneIndex);
    }

    private void Update() {
        // attempt to get the TileGrid for the current scene; if it's null we are done (may be null if it hasn't loaded yet)
        if (!tileGrids.TryGetValue(CurrentScene, out TileGrid currentTileGrid)) return;
        
        // get camera's position
        float cameraX = cameraTransform.position.x;

        if (cameraX < currentTileGrid.XMin && 0 <= currentSceneIndex - 1) {
            // camera has moved outside current scene boundary (LEFT) and new index is still in range; update scene index and unload unused scenes
            currentSceneIndex--;
            UnloadUnusedScenes();
        } else if (cameraX > currentTileGrid.XMax && currentSceneIndex + 1 < SceneNames.Length) {
            // camera has moved outside current scene boundary (RIGHT) and new index is still in range; update scene index and unload unused scenes
            currentSceneIndex++;
            UnloadUnusedScenes();
        }
        
        // attempt to get the TileGrid again for the current scene (current scene may have changed)
        if (!tileGrids.TryGetValue(CurrentScene, out currentTileGrid)) return;

        // if player is within range of next scene (LEFT); load scene
        if (cameraX < currentTileGrid.XMin + proximityUnits) LoadScene(currentSceneIndex - 1);
        
        // if player is within range of next scene (RIGHT); load scene
        if (cameraX > currentTileGrid.XMax - proximityUnits) LoadScene(currentSceneIndex + 1);
    }
    
    //::::::::::::::::::::::::::::://
    // Gizmo Callbacks
    //::::::::::::::::::::::::::::://

    private void OnDrawGizmos() {
        // we only want to register for tilemap events once
        if (registeredTilemapEvents) return;

        // update flag
        registeredTilemapEvents = true;
        
        // register for Tilemap events (once)
        Tilemap.tilemapPositionsChanged += Tilemap_TilemapPositionsChanged;
        Tilemap.tilemapTileChanged += Tilemap_TilemapTileChanged;
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
        if (sceneIndex < 0 || sceneIndex >= SceneNames.Length) return;
        
        // if scene is not unloaded; ignore call
        if (loadStatus[sceneIndex] != LoadStatus.unloaded) return;
        
        // load scene
        StartCoroutine(LoadSceneAdditiveCO(sceneIndex));
    }

    private void UnloadUnusedScenes() {
        for (int i = 0; i < SceneNames.Length; i++) {
            // this scene is not loaded; ignore it
            if (loadStatus[i] != LoadStatus.loaded) continue;
            
            // if this scene is no longer in range; start coroutine to unload it
            if (i < currentSceneIndex - 1 || i > currentSceneIndex + 1) StartCoroutine(UnloadSceneAdditiveCO(i));
        }
    }

    public void UnloadAll()
    {
        for (int i = 0; i < SceneNames.Length; i++)
        {
            // this scene is not loaded; ignore it
            if (loadStatus[i] != LoadStatus.loaded) continue;

            // if this scene is no longer in range; start coroutine to unload it
            StartCoroutine(UnloadSceneAdditiveCO(i));
        }
    }
    
    //:::::::::::::::::::::::::::::://
    // Coroutines
    //:::::::::::::::::::::::::::::://

    private IEnumerator LoadSceneAdditiveCO(int sceneIndex) {
        // update status to LOADING
        loadStatus[sceneIndex] = LoadStatus.loading;
        
        // start loading scene async (additive)
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneNames[sceneIndex], LoadSceneMode.Additive);
        
        // wait until scene has loaded
        while (!asyncOperation.isDone) yield return null;
        
        // update status to LOADED
        loadStatus[sceneIndex] = LoadStatus.loaded;
        FirstSceneLoaded = true;
    }

    private IEnumerator UnloadSceneAdditiveCO(int sceneIndex) {
        // update status UNLOADING
        loadStatus[sceneIndex] = LoadStatus.unloading;
        
        // start unloading scene async
        AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(SceneNames[sceneIndex], UnloadSceneOptions.None);
        
        // wait until scene has unloaded
        while (!asyncOperation.isDone) yield return null;
        
        // update status to UNLOADED
        loadStatus[sceneIndex] = LoadStatus.unloaded;
    }
    
    //:::::::::::::::::::::::::::::://
    // Tilemap Events
    //:::::::::::::::::::::::::::::://

    private void Tilemap_TilemapPositionsChanged(Tilemap tilemap, NativeArray<Vector3Int> positions) {
        tilemap.CompressBounds();
    }

    private void Tilemap_TilemapTileChanged(Tilemap tilemap, Tilemap.SyncTile[] syncTiles) {
        tilemap.CompressBounds();
    }
}
