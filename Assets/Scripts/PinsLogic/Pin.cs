using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Pin : MonoBehaviour
{
    public event UnityAction Fallen;
    [SerializeField] private float angleForFall;
    [SerializeField] private float waitSecondsForFall;
    private bool _isFallen;
    private bool _isCollision;
    private void OnCollisionEnter(Collision collision)
    {
        _isCollision = true;
    }
    private void Update()
    {
        if(!_isCollision)
            transform.rotation = Quaternion.Euler(Vector3.zero);
        if (_isFallen)
            return;
        bool isXFallen = Mathf.Abs(transform.rotation.eulerAngles.x) > angleForFall && Mathf.Abs(transform.rotation.eulerAngles.x) < 180 || Mathf.Abs(transform.rotation.eulerAngles.x-360) > angleForFall && Mathf.Abs(transform.rotation.eulerAngles.x) > 180;
        bool isZFallen = Mathf.Abs(transform.rotation.eulerAngles.z) > angleForFall && Mathf.Abs(transform.rotation.eulerAngles.z) < 180 || Mathf.Abs(transform.rotation.eulerAngles.z-360) > angleForFall && Mathf.Abs(transform.rotation.eulerAngles.z) > 180;
        _isFallen = isXFallen|| isZFallen;
        if (_isFallen)
        {
            Fallen?.Invoke();
        }
    }
}
