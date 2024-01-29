using System;
using UnityEngine;
using UnityEngine.Serialization;

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
    // Unity Callbacks
    //::::::::::::::::::::::::::::://

    private void Start() {
        // if a ParallaxLayer prefab is not set; we are done
        if (!parallaxLayerPrefab) return;

        foreach (var data in parallaxData) {
            // if the data layer is not enabled; just ignore it
            if (!data.enabled) continue;
            
            // instantiate a new ParallaxLayer for each ParallaxData and configure it
            ParallaxLayer parallaxLayer = Instantiate(parallaxLayerPrefab, transform);
            parallaxLayer.Configure(data);
        }
    }
}

//+++++++++++++++++++++++++++++//
// CLASS: ParallaxData
//+++++++++++++++++++++++++++++//

[Serializable]
public class ParallaxData {
    [Tooltip("If disabled, layer will not be shown when game is playing.")]
    public bool enabled = true;
    [Tooltip("0 = Stationary image. 1 = Same speed as camera.")]
    public float moveSpeed = 1f;
    [Tooltip("Negative values will display behind the player; positive values in front.")]
    public int sortingOrder;
    public ParallaxSpriteData[] spriteData;
}

//+++++++++++++++++++++++++++++//
// CLASS: ParallaxSpriteData
//+++++++++++++++++++++++++++++//

[Serializable]
public class ParallaxSpriteData {
    [Tooltip("The sprite to display.")]
    public Sprite sprite;
    [Tooltip("How many times the sprite will repeat before moving to the next one.")]
    public int count = 1;
}