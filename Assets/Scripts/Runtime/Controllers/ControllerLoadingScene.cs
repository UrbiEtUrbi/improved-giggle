using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerLoadingScene : MonoBehaviour
{
    void Start()
    {
        ControllerGameFlow.Instance.LoadNewScene("GameScene");
    }
}
