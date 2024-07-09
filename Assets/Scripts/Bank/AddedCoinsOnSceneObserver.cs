using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AddedCoinsOnSceneObserver : MonoBehaviour
{
    [SerializeField] private PinObserver pinObserver;
    private int amount;
    public int Amount => amount;
    private void Start()
    {
        pinObserver.PinFallen += () => amount++;
    }

}
