using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PCInputService : MonoBehaviour, IInputService
{
    public event UnityAction<Vector2> InputDirection;
    [SerializeField] private float minimum; 
    [SerializeField] private float delay; 
    private Vector2 startDragPosition;
    private bool _isAfterDelay;
    bool IInputService.CanInput { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(delay);
        _isAfterDelay = true;
    }
    private void Update()
    {
        if(!_isAfterDelay)
            return;
        // Проверяем, работает ли код на PC (не на мобильном устройстве)
#if !UNITY_ANDROID && !UNITY_IOS && !UNITY_IPHONE
        HandlePcInput();
#else
    HandleMobileInput();
#endif
    }

    private void HandlePcInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startDragPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            CalculateAndSendDirection(Input.mousePosition);
        }
    }

    private void HandleMobileInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startDragPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                CalculateAndSendDirection(touch.position);
            }
        }
    }
    private void CalculateAndSendDirection(Vector3 endPosition)
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 normalizedEndPosition = new Vector2(endPosition.x / screenSize.x, endPosition.y / screenSize.y);
        Vector2 normalizedStartDragPosition = new Vector2(startDragPosition.x / screenSize.x, startDragPosition.y / screenSize.y);

        Vector2 direction = normalizedEndPosition - normalizedStartDragPosition;
        if (direction.magnitude < minimum)
            return;
        InputDirection?.Invoke(direction);
    }
}
