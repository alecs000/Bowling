using TMPro;
using UnityEngine;
using YG;

public class TMPTranslation : MonoBehaviour
{
        [TextArea(1, 100)]
        [SerializeField] private string textRU;

#if RU_YG2
        void Start()
        {
            if (textRU != string.Empty && YG2.envir.language == "ru")
                GetComponent<TMP_Text>().text = textRU;
        }
#endif
}
