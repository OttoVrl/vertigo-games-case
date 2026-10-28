using Runtime.Spin;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class ZoneElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text indexText;

        public void Initialize(ZoneStep step)
        {
            indexText.text = step.Index.ToString();
            indexText.color = step.Data.ZoneColor;
        }

        public void SetEmpty()
        {
            indexText.text = string.Empty;
        }
    }
}
