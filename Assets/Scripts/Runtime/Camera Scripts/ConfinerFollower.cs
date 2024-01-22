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
            ControllerGame.ControllerRespawn.OnPlayerRepawned.AddListener(OnPlayerRespawned);
            var posX = ControllerGame.Player.transform.position.x;
            Confiner = ControllerGame.Instance.GetCCamera.GetComponent<CinemachineConfiner2D>();
            points[2] = new Vector2(posX + 10000f, points[2].y);
            points[3] = new Vector2(posX + 10000f, points[3].y);
            isInitialized = true;
        }

        UpdatePositions(points);
       

    }

    void UpdatePositions(Vector2[] points)
    {
        var returnX = ControllerGame.Player.MovementController.GetMinimumXReturnPosition;


        //move the collider to block the camera from returning
        points[0] = new Vector2(returnX, points[0].y);
        points[1] = new Vector2(returnX, points[1].y);



        polygonCollider.SetPath(0, points);
        //recalculate bounds if we moved
        if (previousX > -1000 && previousX < returnX)
        {
            Confiner.InvalidateCache();
        }

        previousX = returnX;
    }

    void OnPlayerRespawned()
    {
        var points = polygonCollider.points;
        previousX = -999f;
        UpdatePositions(points);
        Confiner.InvalidateCache();
    }
}
