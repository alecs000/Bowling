using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradesService : MonoBehaviour
{
    [SerializeField] private UpgradableItem[] upgradableItems;
    [SerializeField] private UpgradePanel[] UpgradePanels;
    [SerializeField] private int maxUpgradeLevel;

    private List<UpgradeLogic> logics;
    public List<UpgradeLogic> UpgradeLogics => logics;
    private IEnumerator Start()
    {
        yield return null;

        for (int i = 0; i < upgradableItems.Length; i++)
        {
            UpgradeLogic logic = new UpgradeLogic(upgradableItems[i].Hash, maxUpgradeLevel);
            logic.AddObserver(upgradableItems[i].Upgrade);
            UpgradePanels[i].Set(logic);
            if(logic.CurrentUpgradeValue !=-1)
                upgradableItems[i].Upgrade(logic.CurrentUpgradeValue);
        }
    }
    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}
