using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCameraToPins : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    [SerializeField] private CinemachineVirtualCamera pinsCamera;
    [SerializeField] private TriggerZone triggerZone;

    private void Start()
    {
        triggerZone.Triggered += Switch;
    }
    private void Switch()
    {
        mainCamera.gameObject.SetActive(false);
        pinsCamera.gameObject.SetActive(true);
    }
}
