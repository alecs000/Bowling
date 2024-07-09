using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OpenShopExecutor : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private PanelAnimation panelAnimation;
    public void OnPointerClick(PointerEventData eventData)
    {
        panelAnimation.Switch();
    }
}
