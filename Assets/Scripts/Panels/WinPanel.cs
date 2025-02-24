using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PlayerPrefs = RedefineYG.PlayerPrefs;

public class WinPanel : EndGamePanel
{
    [SerializeField] private TMP_Text coins;
    [SerializeField] private AddedCoinsOnSceneObserver addedCoinsOnSceneObserver;

    private void Awake()
    {
        coins.text = addedCoinsOnSceneObserver.Amount.ToString();
    }
    private void OnEnable()
    {
        PlayerPrefs.Save();
    }
}
