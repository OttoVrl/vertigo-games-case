using Runtime.Spin;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class WheelSliceView : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text amountText;
        [SerializeField] private Sprite bombSprite;

        public void Initialize(RolledSlice sliceData)
        {
            iconImage.sprite = sliceData.IsBomb ? bombSprite : sliceData.collectibleData.Icon;
            amountText.gameObject.SetActive(!sliceData.IsBomb);
            
            if(!sliceData.IsBomb) amountText.text = $"x{sliceData.Amount}";
        }
    }
}
