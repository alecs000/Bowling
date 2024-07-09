using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PCInputService : MonoBehaviour, IInputService
{
    public event UnityAction<Vector2> InputDirection;
    private Vector2 startDragPosition;

    bool IInputService.CanInput { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }


    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            startDragPosition = Input.mousePosition;
        }
        else if(Input.GetMouseButtonUp(0)) {
            Vector2 direction = (Vector2)Input.mousePosition - startDragPosition;
            InputDirection?.Invoke(direction);
        }
    }

}
