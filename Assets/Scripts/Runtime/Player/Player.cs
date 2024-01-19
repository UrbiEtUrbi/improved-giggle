using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{


    public PlayerMovementController MovementController;

    private void Awake()
    {
        MovementController = GetComponent<PlayerMovementController>();
    }
}
