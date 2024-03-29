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

    [BeginGroup("Settings")]
    [EndGroup]
    [SerializeField]
    Vector3 StartPosition = new Vector3(-9.7f, -5.4f, 0);


    public CinemachineVirtualCamera GetCCamera => VCamera;

    [HideInInspector]
    public static Player Player => Instance.player;

    Player player;

    public bool IsGameOver;



    #region Controllers
    ControllerEntities m_ControllerEntities;
    public static ControllerEntities ControllerEntities => Instance.m_ControllerEntities;

    SceneLoader m_SceneLoader;
    public static SceneLoader SceneLoader => Instance.m_SceneLoader;


    ControllerRespawn m_ControllerRespawn;
    public static ControllerRespawn ControllerRespawn => Instance.m_ControllerRespawn;

    ScreenFader m_Fader;
    public static ScreenFader Fader => Instance.m_Fader;

    ControllerAttack m_ControllerAttack;
    public static ControllerAttack ControllerAttack => Instance.m_ControllerAttack;

    ControllerDialog m_ControllerDialog;
    public static ControllerDialog ControllerDialog => Instance.m_ControllerDialog;

    ControllerTimer m_ControllerTimer;
    public static ControllerTimer ControllerTimer => Instance.m_ControllerTimer;

    #endregion

    public static bool Initialized
    {
        get
        {
            if (!Instance)
            {
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
       
     

        m_ControllerEntities = GatherComponent<ControllerEntities>();
        m_SceneLoader = GetComponent<SceneLoader>();
        m_ControllerRespawn = GatherComponent<ControllerRespawn>();
        m_Fader = GatherComponent<ScreenFader>();
        m_ControllerAttack = GetComponent<ControllerAttack>();
        m_ControllerDialog = GetComponent<ControllerDialog>();
        m_ControllerTimer = GetComponent<ControllerTimer>();

        m_ControllerTimer.timing = false;
        Instance = this;



        // MusicPlayer.Instance.PlayPlaylist("overworld");



        base.Init();
        m_ControllerDialog.Init();
        StartCoroutine(WaitForSceneLoad());
    }

    IEnumerator WaitForSceneLoad() {
        yield return new WaitUntil(() => m_SceneLoader.FirstSceneLoaded);
        player = Instantiate(PlayerPrefab);

        player.transform.position = StartPosition;
        player.IsAlive = true;
        VCamera.Follow = player.transform;
       
       
    }

    public void GameOver()
    {
        player.MovementController.SetTrigger("IsTransforming");
        ControllerDialog.ResetTargets();
        ControllerDialog.TriggerDialogue("Transform", player.transform, new Vector3(0, 2, 0));
        IsGameOver = true;

        //reload whole game scenes or entities
        SoundManager.Instance.PlayDelayed("transform", 3f); 
        m_Fader.StartFade(10f, true, 10f);

        Invoke(nameof(Reload), 9.5f);
    }

  

    void Reload()
    {
        MusicPlayer.Instance.StopPlaying(1);
        SceneLoader.UnloadAll();
        ControllerGameFlow.Instance.ResetCurrentScene();

    }

    public void PlayerDie()
    {
        //reload scenes or entities
        m_Fader.StartFade(0.5f,true,0.5f);

        m_ControllerRespawn.Respawn(0.5f+ m_Fader.TimeToFade);
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
