using UnityEngine;

public class ParallaxLayer : MonoBehaviour {
    
    //::::::::::::::::::::::::::::://
    // Local Fields
    //::::::::::::::::::::::::::::://
    
    private float spriteWidth, xTotal;
    private SpriteRenderer[] spriteRenderers;
    
    //::::::::::::::::::::::::::::://
    // Unity Callbacks
    //::::::::::::::::::::::::::::://

    private void Awake() {
        // get sprite renderer components (should be three)
        spriteRenderers = transform.GetComponentsInChildren<SpriteRenderer>();
    }
    
    //-----------------------------//
    // Configuration
    //-----------------------------//
    
    public void Configure(Sprite backgroundSprite, int index) {
        // get the width of the sprite (used in calcs)
        spriteWidth = backgroundSprite.bounds.size.x;
        
        // calculate the starting position of the first sprite
        float x = (spriteRenderers.Length - 1f) / 2f * -spriteWidth;

        foreach (var spriteRenderer in spriteRenderers) {
            // update background sprite and move into position
            spriteRenderer.sprite = backgroundSprite;
            spriteRenderer.transform.position = new Vector3(x, 0f, 0f);
            
            // update the order so it renders correctly
            spriteRenderer.sortingOrder = index;
            
            // increment x for next sprite
            x += spriteWidth;
        }
    }
    
    //-----------------------------//
    // Movement
    //-----------------------------//

    public void Move(float x, float xSpeed) {
        // calculate how far to move background (does not include camera movement)
        float moveX = -x * xSpeed;
        
        // increment total
        xTotal += moveX;
        
        // reset movement to provide infinite scrolling
        if (spriteWidth < xTotal) {
            xTotal -= spriteWidth;
            moveX -= spriteWidth;
        } else if (xTotal < -spriteWidth) {
            xTotal += spriteWidth;
            moveX += spriteWidth;
        }
        
        // move sprites (include camera movement)
        foreach (var spriteRenderer in spriteRenderers) spriteRenderer.transform.position += Vector3.right * (x + moveX);
    }
}