using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerAttack : MonoBehaviour
{


    [SerializeField]
    List<AttackData> Attacks;


    public void Attack(Transform source, bool parentToSource, AttackType Type, Vector3 position, Vector3 size, int damage,float lifetime = -1)
    {
        var data = Attacks.Find(x => x.AttackType == Type);
        var attack = Instantiate(data.AttackObject, position, default, parentToSource ? source : null);
        attack.Init(size, position, damage, lifetime);

        if (data.AttackVFX != null)
        {
            var attackVfx = Instantiate(data.AttackVFX, position, default, parentToSource ? source : null);
        }
    }


    
}

[System.Serializable]
public class AttackData{

    [BeginGroup]
    public AttackType AttackType;
    public AttackObject AttackObject;
    [EndGroup]
    public GameObject AttackVFX;
}

public enum AttackType
{
    PlayerSword,
}