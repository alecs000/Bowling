using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeLogic
{
    private string _hash;
    protected ObservableVariable<int> _upgradeValue;
    public int CurrentUpgradeValue => _upgradeValue.Value;
    public int MaxUpgradeValue;
    public UpgradeLogic(string hash, int maxUpgradeValue)
    {
        _hash = hash;
        MaxUpgradeValue = maxUpgradeValue;

        int amountUpgrades = PlayerPrefs.GetInt(_hash, -1);
        _upgradeValue = new ObservableVariable<int>(amountUpgrades);
    }

    public bool TryUpgrade()
    {
        if(MaxUpgradeValue-1<= _upgradeValue.Value)
            return false;
        _upgradeValue.Value ++;
        PlayerPrefs.SetInt(_hash, _upgradeValue.Value);
        return true;
    }
    public void AddObserver(Action<int> @delegate)
    {
        _upgradeValue.OnChanged += @delegate;
    }

    public void RemoveObserver(Action<int> @delegate)
    {
        _upgradeValue.OnChanged -= @delegate;
    }
}
