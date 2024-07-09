using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UpgradableItem : MonoBehaviour
{
    [SerializeField] private string hash;
    public string Hash=>hash;
    public abstract void Upgrade(int level);
} 
