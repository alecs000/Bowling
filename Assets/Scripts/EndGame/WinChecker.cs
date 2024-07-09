using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WinChecker : MonoBehaviour
{
    public event UnityAction Win;
    [SerializeField] private PinsGenerator pinsGenerator;
    [SerializeField] private float winDelay;
    private bool _isFirstPinFallen;
    private void Awake()
    {
        pinsGenerator.PinsGenerated += Initialize;
    }
    private void Initialize()
    {
        for (int i = 0; i < pinsGenerator.Pins.Count; i++)
        {
            pinsGenerator.Pins[i].Fallen += OnPinFallen;
        }
    }
    private void OnPinFallen()
    {
        if(_isFirstPinFallen)
            return;
        _isFirstPinFallen = true;
        StartCoroutine(DelayForWin());
    }
    private IEnumerator DelayForWin()
    {
        yield return new WaitForSeconds(winDelay);
        Win?.Invoke();
    }
}
