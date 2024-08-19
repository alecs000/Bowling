using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PCInputService : MonoBehaviour, IInputService
{
    public event UnityAction<Vector2> InputDirection;
    [SerializeField] private float minimum; 
    private Vector2 startDragPosition;

    bool IInputService.CanInput { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }


    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            startDragPosition = Input.mousePosition;
        }
        else if(Input.GetMouseButtonUp(0)) {
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            Vector2 normalizedMousePosition = new Vector2(Input.mousePosition.x / screenSize.x, Input.mousePosition.y / screenSize.y);
            Vector2 normalizedStartDragPosition = new Vector2(startDragPosition.x / screenSize.x, startDragPosition.y / screenSize.y);

            Vector2 direction = normalizedMousePosition - normalizedStartDragPosition;
            if (direction.magnitude < minimum)
                return;
            InputDirection?.Invoke(direction);
        }
    }

}
