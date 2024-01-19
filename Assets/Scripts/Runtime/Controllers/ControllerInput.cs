using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ControllerInput : GenericSingleton<ControllerInput>
{
    [HideInInspector]
    public UnityEvent<float> Horizontal = new UnityEvent<float>();
    [HideInInspector]
    public UnityEvent<float> Vertical = new UnityEvent<float>();

    [HideInInspector]
    public UnityEvent<bool> Jump = new UnityEvent<bool>();
    [HideInInspector]
    public UnityEvent Attack = new UnityEvent();


    void OnAttack(InputValue inputValue)
    {
        if (inputValue.Get<float>() > 0)
        {
            Attack.Invoke();
        }
    }

    void OnJump(InputValue inputValue)
    {
        Debug.Log($"jump");
        Jump.Invoke(inputValue.Get<float>() > 0);
    }

    void OnHorizontal(InputValue inputValue)
    {
        var horizontalInputRaw = inputValue.Get<float>();
        Horizontal.Invoke(horizontalInputRaw);

    }

    void OnVertical(InputValue inputValue)
    {
        var vertInputRaw = inputValue.Get<float>();
        Vertical.Invoke(vertInputRaw);

    }

    
   
}
