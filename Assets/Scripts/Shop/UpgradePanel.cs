using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    [SerializeField] private UpgradeProgression upgradeProgression;
    [SerializeField] private Button button;
    [SerializeField] private CoinsService coinsService;
    [SerializeField] private int cost;

    private UpgradeLogic _logic;
    private void Start()
    {
        button.onClick.AddListener(OnClick);
    }
    public void Set(UpgradeLogic logic)
    {
        _logic = logic;
        upgradeProgression.ShowProgress(_logic.CurrentUpgradeValue);
    }
    private void OnClick()
    {
        if (coinsService.Value>=cost&& _logic.TryUpgrade())
        {
            coinsService.TryRemove(cost);
            upgradeProgression.ShowProgress(_logic.CurrentUpgradeValue);
        }
    }
}
