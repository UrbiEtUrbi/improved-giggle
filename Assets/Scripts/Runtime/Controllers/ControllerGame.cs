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

    [HideInInspector]
    public Player Player;

    public override void Init()
    {
        Player = Instantiate(PlayerPrefab);
        VCamera.Follow = Player.transform;

        base.Init();    
    }
}
