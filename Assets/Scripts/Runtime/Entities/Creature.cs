using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : Entity, IHealth
{



    public void ChangeHealth(int amount)
    {

        if (amount < 0)
        {
            //spawn damage vfx

            //1 cut 1 kill
            Die();
        }
    }

    public void Die()
    {
        //spawn death animation
        Destroy(gameObject);
    }

    public void SetInitialHealth(int amount)
    {
       
    }
}
