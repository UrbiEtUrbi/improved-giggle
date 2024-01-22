using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGrid : MonoBehaviour {
    
    //------------------------------//
    // Properties
    //------------------------------//

    public int XMin => tilemap ? tilemap.cellBounds.xMin + Mathf.RoundToInt(transform.position.x) : 0;
    public int XMax => tilemap ? tilemap.cellBounds.xMax + Mathf.RoundToInt(transform.position.x) : 0;
    
    //::::::::::::::::::::::::::::://
    // Local Fields
    //::::::::::::::::::::::::::::://
    
    private Grid grid;
    private Tilemap tilemap;
    
    //::::::::::::::::::::::::::::://
    // Unity Callbacks
    //::::::::::::::::::::::::::::://

    private void Awake() {
        GetAllComponents();
    }

    private void OnEnable() {
        // register TileGrid with SceneLoader so it can adjust it's position as needed
        SceneLoader.Instance.RegisterTileGrid(this);
    }

    private void OnDisable() {
        // deregister TileGrid from SceneLoader
        SceneLoader.Instance.DeregisterTileGrid(this);
    }
    
    //------------------------------//
    // Resetting Position
    //------------------------------//

    public void ResetXMin(int xMin) {
        // if we failed to get all components; we are done
        if (!GetAllComponents()) return;
        
        // reset the position so cell bounds butt up against previous scene
        transform.position = Vector3.right * (xMin - tilemap.cellBounds.xMin);
    }

    //::::::::::::::::::::::::::::://
    // Utilities
    //::::::::::::::::::::::::::::://

    private bool GetAllComponents() {
        // if we have already retrieved the components; we are done (success)
        if (grid && tilemap) return true;
        
        // attempt to get the Grid component; if it's not returned; we are done (fail)
        grid = GetComponent<Grid>();
        if (!grid) return false;
        
        // attempt to get the Tilemap component; if it's not returned; we are done (fail)
        tilemap = grid.GetComponentInChildren<Tilemap>();
        if (!tilemap) return false;
        
        // compress bounds (this will resize the tilemap to an area that only has tiles)
        tilemap.CompressBounds();
        
        // we are done (success)
        return true;
    }
    
    //::::::::::::::::::::::::::::://
    // Gizmos
    //::::::::::::::::::::::::::::://

    private void OnDrawGizmos() {
        // if we failed to get all components; we are done
        if (!GetAllComponents()) return;
        
        // get the TileGrid position and TileMap bounds (TileMap will not be null)
        Vector3 position = transform.position;
        BoundsInt bounds = tilemap.cellBounds;
        
        // get each corner position (ignore z value)
        Vector3 topLeft = new(bounds.xMin + position.x, bounds.yMax + position.y, 0);
        Vector3 topRight = new(bounds.xMax + position.x, bounds.yMax + position.y, 0);
        Vector3 botLeft = new(bounds.xMin + position.x, bounds.yMin + position.y, 0);
        Vector3 botRight = new(bounds.xMax + position.x, bounds.yMin + position.y, 0);
        
        // draw magenta box around tilemap area
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(topLeft, topRight); // top
        Gizmos.DrawLine(topRight, botRight); // right
        Gizmos.DrawLine(botRight, botLeft); // bottom
        Gizmos.DrawLine(botLeft, topLeft); // left
    }
}
