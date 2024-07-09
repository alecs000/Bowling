using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeProgression : MonoBehaviour
{
    [SerializeField] private Image[] progressImages;

    public void ShowProgress(int amount)
    {
        for (int i = 0; i < progressImages.Length; i++)
        {
            progressImages[i].gameObject.SetActive(amount >= i);
        }
    }
}
