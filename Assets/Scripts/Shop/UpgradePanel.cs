using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    [SerializeField] private UpgradeProgression upgradeProgression;
    [SerializeField] private Button button;
    [SerializeField] private CoinsService coinsService;
    [SerializeField] private int cost;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Image complete;

    private int _currentCost;
    private UpgradeLogic _logic;
    private void Start()
    {
        button.onClick.AddListener(OnClick);
    }
    public void Set(UpgradeLogic logic)
    {
        _logic = logic;
        upgradeProgression.ShowProgress(_logic.CurrentUpgradeValue);
        _currentCost = cost * (_logic.CurrentUpgradeValue+2);
        costText.text = _currentCost.ToString();
        if (_logic.MaxUpgradeValue == _logic.CurrentUpgradeValue + 1)
        {
            costText.gameObject.SetActive(false);
            complete.gameObject.SetActive(true);
        }
    }
    private void OnClick()
    {
        if (coinsService.Value>=_currentCost&& _logic.TryUpgrade())
        {
            coinsService.TryRemove(_currentCost);
            upgradeProgression.ShowProgress(_logic.CurrentUpgradeValue);
            _currentCost = cost * (_logic.CurrentUpgradeValue + 2);
            costText.text = _currentCost.ToString();
            if (_logic.MaxUpgradeValue == _logic.CurrentUpgradeValue+1)
            {
                costText.gameObject.SetActive(false);
                complete.gameObject.SetActive(true);
            }
        }
    }
}

