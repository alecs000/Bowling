using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IInputService
{
    public event UnityAction<Vector2> InputDirection;
    public bool CanInput { get; protected set; }
}
