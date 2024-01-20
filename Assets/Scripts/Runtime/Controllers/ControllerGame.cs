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
    public static Player Player => Instance.player;

    Player player;


    #region Controllers
    ControllerEntities m_ControllerEntities;
    public static ControllerEntities ControllerEntities => Instance.m_ControllerEntities;

    #endregion

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
        player = Instantiate(PlayerPrefab);
        VCamera.Follow = Player.transform;
        Instance = this;

        m_ControllerEntities = GatherComponent<ControllerEntities>();

        base.Init();    
    }

    public T GatherComponent<T>() where T : MonoBehaviour {
        var component = GetComponent<T>();
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }
        return component;
    }
}
