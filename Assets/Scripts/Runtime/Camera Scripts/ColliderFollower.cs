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
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!ControllerGame.Initialized)
        {
            return;
        }
       
       


        var returnX = ControllerGame.Instance.Player.MovementController.GetMinimumXReturnPosition;

        transform.position = new Vector3(returnX - boxCollider.bounds.extents.x - boxCollider.edgeRadius, 0, 0);
        

    }
}
