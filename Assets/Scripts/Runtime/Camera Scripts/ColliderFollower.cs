using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderFollower : MonoBehaviour
{
    BoxCollider2D boxCollider;    
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }
    void FixedUpdate()
    {
        if (!ControllerGame.Initialized)
        {
            return;
        }
        var returnX = ControllerGame.Instance.Player.MovementController.GetMinimumXReturnPosition;

        //move the collider to block the player from returning
        transform.position = new Vector3(returnX - boxCollider.bounds.extents.x - boxCollider.edgeRadius, 0, 0);
    }
}
