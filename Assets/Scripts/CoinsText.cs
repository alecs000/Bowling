using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinsText : MonoBehaviour
{
    [SerializeField] private TMP_Text coins;
    [SerializeField] private CoinsService coinsService;

    private void Start()
    {
        coinsService.AddObserver((int value)=>coins.text = value.ToString());
        coins.text = coinsService.Value.ToString();
    }
}
