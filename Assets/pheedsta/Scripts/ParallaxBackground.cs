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
    public bool enabled = true;
    public float moveSpeed = 1f;
    public int sortingOrder;
    public ParallaxSpriteData[] spriteData;
}

//+++++++++++++++++++++++++++++//
// CLASS: ParallaxSpriteData
//+++++++++++++++++++++++++++++//

[Serializable]
public class ParallaxSpriteData {
    public Sprite sprite;
    public int count = 1;
}