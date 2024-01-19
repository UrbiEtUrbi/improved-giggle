using System;
using UnityEngine;

//+++++++++++++++++++++++++++++//
// CLASS: ParallaxBackground
//+++++++++++++++++++++++++++++//

public class ParallaxBackground : MonoBehaviour {

    //::::::::::::::::::::::::::::://
    // Serialized Fields
    //::::::::::::::::::::::::::::://

    [SerializeField] private ParallaxLayer parallaxLayerPrefab;
    [SerializeField] private ParallaxData[] parallaxData;

    //::::::::::::::::::::::::::::://
    // Local Fields
    //::::::::::::::::::::::::::::://

    private Transform cameraTransform;
    private ParallaxLayer[] parallaxLayers;
    private Vector3 previousCameraPosition;
    
    //::::::::::::::::::::::::::::://
    // Unity Callbacks
    //::::::::::::::::::::::::::::://

    private void Awake() {
        // get main camera transform
        Camera mainCamera = Camera.main;
        if (mainCamera != null) cameraTransform = mainCamera.transform;
    }

    private void Start() {
        // if a ParallaxLayer prefab is not set; we are done
        if (!parallaxLayerPrefab) return;

        // initialise array
        parallaxLayers = new ParallaxLayer[parallaxData.Length];

        for (int i = 0; i < parallaxData.Length; i++) {
            // instantiate new parallax layer and configure
            ParallaxLayer parallaxLayer = Instantiate(parallaxLayerPrefab, transform);
            parallaxLayer.Configure(parallaxData[i].backgroundSprite, i);
            
            // add new layer to array
            parallaxLayers[i] = parallaxLayer;
        }
        
        // get the start position of the camera
        previousCameraPosition = cameraTransform.position;
    }

    private void Update() {
        // check that data and layers have the same count (this should never fail)
        if (parallaxData.Length != parallaxLayers.Length) return;
        
        // get the distance moved by the camera and update previous position
        float cameraX = cameraTransform.position.x - previousCameraPosition.x;

        // move each layer by the horizontal speed provided
        for (int i = 0; i < parallaxData.Length; i++) parallaxLayers[i].Move(cameraX, parallaxData[i].horizontalSpeed);
        
        // update position field (used for calcs next frame)
        previousCameraPosition = cameraTransform.position;
    }
}

//+++++++++++++++++++++++++++++//
// CLASS: ParallaxData
//+++++++++++++++++++++++++++++//

[Serializable]
public class ParallaxData {
    public Sprite backgroundSprite;
    public float horizontalSpeed = 1f;
}