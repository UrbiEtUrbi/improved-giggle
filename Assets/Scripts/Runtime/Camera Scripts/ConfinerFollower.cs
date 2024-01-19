using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// Controller the confiner of the camera, so the player runs to the edge of the camera when returning
/// </summary>
public class ConfinerFollower : MonoBehaviour
{

    PolygonCollider2D polygonCollider;
    CinemachineConfiner2D Confiner;


    float previousX = -1000;
    bool isInitialized = false;
    private void Awake()
    {
        polygonCollider = GetComponent<PolygonCollider2D>();
    }
    // Update is called once per frame
    void Update()
    {
        if (!ControllerGame.Initialized)
        {
            return;
        }
        var points = polygonCollider.points;
        if (!isInitialized)
        {
            Confiner = ControllerGame.Instance.GetCCamera.GetComponent<CinemachineConfiner2D>();
            points[2] = new Vector2(ControllerGame.Instance.Player.transform.position.x + 10000f, points[2].y);
            points[3] = new Vector2(ControllerGame.Instance.Player.transform.position.x + 10000f, points[3].y);
            isInitialized = true;
        }
        
       
        var returnX = ControllerGame.Instance.Player.MovementController.GetMinimumXReturnPosition;
        points[0] = new Vector2(returnX, points[0].y);
        points[1] = new Vector2(returnX, points[1].y);

       

        polygonCollider.SetPath(0, points);
        if (previousX > -1000 && previousX < returnX)
        {
            Confiner.InvalidateCache();
        }

        previousX = returnX;

    }
}
