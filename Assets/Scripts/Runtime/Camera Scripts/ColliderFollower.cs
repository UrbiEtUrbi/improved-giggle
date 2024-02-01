using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderFollower : MonoBehaviour
{
    BoxCollider2D boxCollider;

    bool initialized = false;
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

        if (!initialized)
        {
            ControllerGame.ControllerRespawn.OnPlayerRepawned.AddListener(OnPlayerRespawned);
            initialized = true;
        }
        UpdatePosition();
    }

    void OnPlayerRespawned() {
        UpdatePosition();
    }

    void UpdatePosition()
    {
        if (ControllerGame.Player == null)
        {
            return;
        }
        var returnX = ControllerGame.Player.MovementController.GetMinimumXReturnPosition;
        //move the collider to block the player from returning
        transform.position = new Vector3(returnX - boxCollider.bounds.extents.x - boxCollider.edgeRadius, 0, 0);
    }

    private void OnDisable()
    {   if (ControllerGame.Initialized)
        {
            ControllerGame.ControllerRespawn.OnPlayerRepawned.RemoveListener(OnPlayerRespawned);
        }
    }
}
