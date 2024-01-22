using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(SceneLoader))]
public class SceneLoaderEditor : Editor {
    
    //::::::::::::::::::::::::::::://
    // Constants
    //::::::::::::::::::::::::::::://
    
    private const float buttonWidth = 100f;
    private const float buttonHeight = 50f;
    
    //::::::::::::::::::::::::::::://
    // Readonly Fields
    //::::::::::::::::::::::::::::://
    
    private readonly Dictionary<string, Scene> openedScenes = new();
    
    //::::::::::::::::::::::::::::://
    // Local Fields
    //::::::::::::::::::::::::::::://

    private SceneLoader sceneLoader;
    
    //::::::::::::::::::::::::::::://
    // Editor Callbacks
    //::::::::::::::::::::::::::::://
    
    public override void OnInspectorGUI() {
        // get the SceneLoader this editor is refering to
        sceneLoader = (SceneLoader)target;
        
        // subscribe to events
        EditorSceneManager.sceneClosed += EditorSceneManager_SceneClosed;
        
        // show the SerializeFields at the top of the inspector then show my custom buttons under the SerializeFields
        DrawDefaultInspector();
        DrawCustomInspector();
    }

    private void EditorSceneManager_SceneClosed(Scene scene) {
        // remove the scene from the dictionary
        openedScenes.Remove(scene.name);
    }
    
    //::::::::::::::::::::::::::::://
    // Button Methods
    //::::::::::::::::::::::::::::://

    private void OpenScenes() {
        foreach (var scene in sceneLoader.Scenes) {
            // if this scene is already open; ignore
            if (openedScenes.ContainsKey(scene.name)) continue;
            
            // open scene (additive) and add it to the dictionary
            openedScenes[scene.name] = EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scene), OpenSceneMode.Additive);
        }
    }

    private void RepositionScenes() {
        Debug.Log("RepositionScenes");
    }

    private void SaveScenes() {
        // ask user if they want to save modified scenes
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
    }
    
    //::::::::::::::::::::::::::::://
    // Drawing Methods
    //::::::::::::::::::::::::::::://

    private void DrawCustomInspector() {
        // add some space between default and custom
        GUILayout.Space(10);

        // show buttons
        if (GUILayout.Button("Open\nScenes", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight))) OpenScenes();
        if (GUILayout.Button("Reposition\nScenes", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight))) RepositionScenes();
        if (GUILayout.Button("Save\nScenes", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight))) SaveScenes();
    }
}
