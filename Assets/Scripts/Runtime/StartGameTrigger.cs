using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {

            ControllerGame.ControllerTimer.timing = true;
            MusicPlayer.Instance.PlayPlaylist("overworld");
            Destroy(this.gameObject);

        }
    }
}
