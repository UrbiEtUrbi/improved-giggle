using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


[RequireComponent(typeof(DestroyDelayed))]
public class AttackObject : MonoBehaviour
{
    [SerializeField]
    LayerMask TargetLayer;
    DestroyDelayed dd;

    bool initialized = false;

    Vector2 attackSize;
    int damage;

    [SerializeField]
    bool generateImpulse;
    [SerializeField]
    float impulseAmplitude;
    private void Awake()
    {
        dd = GetComponent<DestroyDelayed>();
     
    }

    public void Init(Vector2 size, Vector3 position, int _damage, float lifetime)
    {
        if (lifetime > 0)
        {
            dd.Init(lifetime);
        }
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
            if (generateImpulse)
            {
                var cis = GetComponent<CinemachineImpulseSource>();
                cis.GenerateImpulse(new Vector3(Random.Range(-1f,1f), Random.Range(-1, 1f), 0) * impulseAmplitude);
            }
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, attackSize);
    }



}
