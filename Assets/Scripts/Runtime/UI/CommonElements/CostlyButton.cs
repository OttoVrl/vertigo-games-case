using System;
using System.Globalization;
using Runtime.Data;
using Runtime.Enums;
using Runtime.Extensions;
using Runtime.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    [RequireComponent(typeof(Button))]
    public class CostlyButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text costText;
        [SerializeField] private Image costTypeImage;
        [SerializeField] private Button button;

        public event Action OnClickSuccess;

        private BaseCollectibleData _type;
        private int cost;
        private bool _isBound;

        public void Initialize(int cost,BaseCollectibleData type)
        {
            _type = type;
            this.cost = cost;

            if (!button) button = GetComponent<Button>();
            if (costText) costText.text = cost.ToString("N0", CultureInfo.InvariantCulture);
            if (costTypeImage && type) costTypeImage.sprite = type.Icon;

            if (_isBound || !button) return;

            button.onClick.AddListener(_OnClickButton);
            _isBound = true;
        }

        public void _OnClickButton()
        {
            SfxManager.Instance.PlaySFX(SFXType.ButtonClick);
            if (_type.Value >= cost)
            {
                SfxManager.Instance.PlaySFX(SFXType.SpendCoin);
                OnClickSuccess?.Invoke();
            }
            else
            {
                Debug.LogWarning("Not enough cost");
            }
        }

        public void Dispose()
        {
            button.onClick.AddListener(_OnClickButton);
            _type = null;
        }

        private void OnValidate()
        {
            if (!button)
                button = this.FindChild<Button>("ui_play_costly_button");
        }
    }
}