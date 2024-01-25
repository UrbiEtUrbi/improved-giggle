using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(DestroyDelayed))]
public class AttackObject : MonoBehaviour
{
    [SerializeField]
    LayerMask TargetLayer;
    DestroyDelayed dd;

    bool initialized = false;

    Vector2 attackSize;
    int damage;

    private void Awake()
    {
        dd = GetComponent<DestroyDelayed>();
     
    }

    public void Init(Vector2 size, Vector3 position, int _damage, float lifetime)
    {
        dd.Init(lifetime);
        attackSize = size;
        damage = _damage;
        transform.position = position;
        initialized = true;
    }


    private void Update()
    {
        if (!initialized) {
            return;
        }
        var colliderHit = Physics2D.OverlapBox(transform.position, attackSize, 0, TargetLayer);
        if (colliderHit != null)
        {
            colliderHit.GetComponent<IHealth>().ChangeHealth(-damage);
            CancelInvoke();
            Destroy(gameObject);
        }
    }



}
