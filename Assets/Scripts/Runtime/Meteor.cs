using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour, IHealth
{

    int health = 8;

    Vector3 pos;

    [SerializeField]
    SpriteRenderer sr;

     void Start()
    {
        pos = transform.position;
    }

    bool dead = false;
    public void ChangeHealth(int amount)
    {
        if (dead)
        {
            return;
        }
        health--;
      
        if (health < 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(Smash());
        }
    }

    public void Die()
    {
        dead = true;
        ControllerGame.ControllerTimer.timing = false;
        ControllerGame.ControllerDialog.TriggerDialogue("End", ControllerGame.Player.transform, new Vector3(0, 2, 0));
        StartCoroutine(Dissapear());
        SoundManager.Instance.CancelAllLoops();
    }

    public void SetInitialHealth(int amount)
    {
      
    }

    int smashFRAMES = 10;

    IEnumerator Smash()
    {
        int frameCount = 0;
        while (true)
        {
            yield return new WaitForSeconds(0.01f);


            transform.position = pos + new Vector3(Random.value, Random.value, 0);
            frameCount++;
            if (frameCount > smashFRAMES)
            {
                yield break;
            }

        }
    }


    float fadeDuration = 2;

    IEnumerator Dissapear()
    {
        float fadeTime = 0;
        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            fadeTime += 0.01f;
            var c = sr.color;
            c.a = (1 - (fadeTime / fadeDuration));
            sr.color = c;
            
            if (fadeTime > fadeDuration)
            {
                Destroy(gameObject);
                yield break;
            }

        }
    }
}
