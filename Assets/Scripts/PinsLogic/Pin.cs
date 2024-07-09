using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pin : MonoBehaviour
{
    public event UnityAction Fallen;
    [SerializeField] private float angleForFall;
    [SerializeField] private float waitSecondsForFall;
    private bool _isFallen;
    private void Update()
    {
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
