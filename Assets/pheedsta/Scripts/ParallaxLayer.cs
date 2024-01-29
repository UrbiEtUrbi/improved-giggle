using System.Collections.Generic;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour {
    
    //::::::::::::::::::::::::::::://
    // Properties
    //::::::::::::::::::::::::::::://

    private float MinCameraX => GetMinCameraX();
    private float MaxCameraX => GetMaxCameraX();
    
    //::::::::::::::::::::::::::::://
    // Constants
    //::::::::::::::::::::::::::::://

    private const float screenMargin = 2f;
    
    //::::::::::::::::::::::::::::://
    // Local Fields
    //::::::::::::::::::::::::::::://

    private int midIndex = 1;
    private float minSpriteRendererX, maxSpriteRendererX, moveSpeed;
    private Vector3 previousCameraPosition;
    
    private Camera mainCamera;
    private Sprite[] sprites;
    private SpriteRenderer[] spriteRenderers;
    
    //::::::::::::::::::::::::::::://
    // Unity Callbacks
    //::::::::::::::::::::::::::::://

    private void Awake() {
        // get sprite renderer components (MUST be three)
        spriteRenderers = transform.GetComponentsInChildren<SpriteRenderer>();
        Debug.Assert(spriteRenderers.Length == 3, "There must be exactly three SpriteRenderer components attached to ParallaxLayer.");
        
        // get the main camera
        mainCamera = Camera.main;
        Debug.Assert(mainCamera, "The main camera could not be found.");
    }

    private void Start() {
        // get the starting position of the camera
        previousCameraPosition = mainCamera.transform.position;
    }

    private void Update() {
        MoveSpriteRenderers();
    }
    
    //-----------------------------//
    // Configuration
    //-----------------------------//
    
    public void Configure(ParallaxData parallaxData) {
        // set move speed (used when calculating how far to move it)
        moveSpeed = parallaxData.moveSpeed;
        
        // initialise an empty List to hold all Sprites in order (we will convert this to an array when done)
        List<Sprite> spriteList = new List<Sprite>();
        
        // add sprites to list
        foreach (var spriteData in parallaxData.spriteData) {
            for (int i = 0; i < spriteData.count; i++) spriteList.Add(spriteData.sprite);
        }
        
        // convert list to array
        sprites = spriteList.ToArray();
        
        // if we don't have at least three Sprites (one for each SpriteRenderer) we are done
        if (sprites.Length < spriteRenderers.Length) return;
        
        // initialise fields used in for loop
        float totalWidth = 0f;
        
        for (int i = 0; i < spriteRenderers.Length; i++) {
            // get the SpriteRenderer and Sprite (we know index will be in range for both arrays)
            SpriteRenderer spriteRenderer = spriteRenderers[i];
            Sprite sprite = sprites[i];
            
            // update SpriteRenderer with Sprite and sorting order
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = parallaxData.sortingOrder;

            // increment total width (we will use this to determine starting positions of SpriteRenderers)
            totalWidth += sprite.bounds.size.x;
        }
        
        // get the camera position
        Vector3 cameraPosition = mainCamera.transform.position;
        
        // calculate the extents of the SpriteRenderers
        minSpriteRendererX = cameraPosition.x + totalWidth / -2f;
        maxSpriteRendererX = minSpriteRendererX + totalWidth;

        // get the starting point (this value will increment)
        float x = minSpriteRendererX;
        
        foreach (SpriteRenderer spriteRenderer in spriteRenderers) {
            // get the sprite width
            float spriteWidth = spriteRenderer.sprite.bounds.size.x;
            
            // update the position of the SpriteRenderer (midpoint)
            spriteRenderer.transform.position = new Vector3(x + spriteWidth / 2f, cameraPosition.y, 0f);
            
            // increment x for start of next sprite
            x += spriteWidth;
        }
    }
    
    //::::::::::::::::::::::::::::://
    // Property Getters
    //::::::::::::::::::::::::::::://

    private float GetMinCameraX() {
        return mainCamera.ViewportToWorldPoint(Vector3.zero).x - screenMargin;
    }

    private float GetMaxCameraX() {
        return mainCamera.ViewportToWorldPoint(Vector3.one).x + screenMargin;
    }
    
    //::::::::::::::::::::::::::::://
    // Moving SpriteRenderers
    //::::::::::::::::::::::::::::://
    
    private void MoveSpriteRenderers() {
        // get the new position of the camera
        Vector3 currentCameraPosition = mainCamera.transform.position;
        
        // get the distances moved by the camera
        float cameraMoveX = currentCameraPosition.x - previousCameraPosition.x;
        float cameraMoveY = currentCameraPosition.y - previousCameraPosition.y;
        
        // update value for the next time
        previousCameraPosition = currentCameraPosition;

        // calculate how far to move background (include camera movement)
        float moveX = -cameraMoveX * moveSpeed + cameraMoveX;
        float moveY = -cameraMoveY * moveSpeed + cameraMoveY;
        
        foreach (var spriteRenderer in spriteRenderers) {
            // get the current position of the SpriteRenderer
            Vector3 position = spriteRenderer.transform.position;
            
            // update X and Y values
            position.x += moveX;
            position.y += moveY;
            
            // reset to new position
            // ReSharper disable once Unity.InefficientPropertyAccess
            spriteRenderer.transform.position = position;
        }
        
        // update SpriteRenderer extents
        minSpriteRendererX += moveX;
        maxSpriteRendererX += moveX;
        
        // shuffle renderers so it is an infinite scroll
        if (MinCameraX < minSpriteRendererX) {
          ShuffleSpriteRenderersLeft();
        } else if (maxSpriteRendererX < MaxCameraX) {
            ShuffleSpriteRenderersRight();
        }
    }

    private void ShuffleSpriteRenderersRight() {
        // if the new mid index is going to be out of range; we are done 
        if (midIndex + 1 >= sprites.Length - 1) return;
        
        // increment mid index
        midIndex++;
        
        // get the FIRST SpriteRenderer (this will be reused and moved to the END of the array) and the NEXT sprite (we know this index will be in range)
        SpriteRenderer reuseSpriteRenderer = spriteRenderers[0];
        Sprite sprite = sprites[midIndex + 1];
        
        // update SpriteRenderer with new sprite
        reuseSpriteRenderer.sprite = sprite;
        
        // initialise fields for cals
        Transform spriteRendererTransform = reuseSpriteRenderer.transform;
        Vector3 position = spriteRendererTransform.position;
        float spriteWidth = sprite.bounds.size.x;
        
        // calculate new position for SpriteRenderer and update
        position.x = maxSpriteRendererX + spriteWidth / 2f;
        spriteRendererTransform.position = position;
        
        // increment left and right extents
        minSpriteRendererX += spriteWidth;
        maxSpriteRendererX += spriteWidth;
        
        // reshuffle the SpriteRenderers (put reused one to the end)
        spriteRenderers[0] = spriteRenderers[1];
        spriteRenderers[1] = spriteRenderers[2];
        spriteRenderers[2] = reuseSpriteRenderer;
    }
    
    private void ShuffleSpriteRenderersLeft() {
        // if the new mid index is going to be out of range; we are done 
        if (midIndex - 1 < 1) return;
        
        // decrement mid index
        midIndex--;
        
        // get the LAST SpriteRenderer (this will be reused and moved to the START of the array) and the PREVIOUS sprite (we know this index will be in range)
        SpriteRenderer reuseSpriteRenderer = spriteRenderers[2];
        Sprite sprite = sprites[midIndex - 1];
        
        // update SpriteRenderer with new sprite
        reuseSpriteRenderer.sprite = sprite;
        
        // initialise fields for cals
        Transform spriteRendererTransform = reuseSpriteRenderer.transform;
        Vector3 position = spriteRendererTransform.position;
        float spriteWidth = sprite.bounds.size.x;
        
        // calculate new position for SpriteRenderer and update
        position.x = minSpriteRendererX - spriteWidth / 2f;
        spriteRendererTransform.position = position;
        
        // decrement left and right extents
        minSpriteRendererX -= spriteWidth;
        maxSpriteRendererX -= spriteWidth;
        
        // reshuffle the SpriteRenderers (put reused one to the start)
        spriteRenderers[1] = spriteRenderers[0];
        spriteRenderers[2] = spriteRenderers[1];
        spriteRenderers[0] = reuseSpriteRenderer;
    }
}