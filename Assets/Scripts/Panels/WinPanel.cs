using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinPanel : EndGamePanel
{
    [SerializeField] private TMP_Text coins;
    [SerializeField] private AddedCoinsOnSceneObserver addedCoinsOnSceneObserver;

    private void Awake()
    {
        coins.text = addedCoinsOnSceneObserver.Amount.ToString();
    }
}
