using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    protected bool isActive;
    public bool IsActive => isActive;

    [SerializeField]
    public float ActivationDistance = 14f;

    [SerializeField]
    public float DeactivationDistance = 20f;



    private void Awake()
    {
        ControllerGame.ControllerEntities.RegisterEntity(this);
    }

    private void OnDestroy()
    {
        ControllerGame.ControllerEntities.UnregisterEntity(this);
    }

    public virtual void ToggleActive(bool _isActive)
    {
        isActive = _isActive;
    }

}
