using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ControllerGame : ControllerLocal
{

    public static ControllerGame Instance;


    [BeginGroup("Prefabs")]
    [EndGroup]
    [SerializeField]
    Player PlayerPrefab;


    [BeginGroup("References")]
    [SerializeField]
    [EndGroup]
    CinemachineVirtualCamera VCamera;


    public CinemachineVirtualCamera GetCCamera => VCamera;

    [HideInInspector]
    public Player Player;

    public static bool Initialized
    {
        get
        {
            if (!Instance)
            {
                Debug.Log("instance not set");
                return false;

            }
            else
            {
                return Instance.isInitialized;
            }
        }

    }

    public override void Init()
    {
        Player = Instantiate(PlayerPrefab);
        VCamera.Follow = Player.transform;
        Instance = this;
        base.Init();    
    }
}
