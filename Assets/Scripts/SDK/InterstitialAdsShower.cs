using UnityEngine;
using UnityEngine.UI;
using YG;

public class InterstitialAdsShower : MonoBehaviour
{
    [SerializeField] private Button show;

    private void Start()
    {
        show.onClick.AddListener(() => YG2.InterstitialAdvShow());
    }
}
