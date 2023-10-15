using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IInputService
{
   public event UnityAction<Vector2> InputDirection;
}
