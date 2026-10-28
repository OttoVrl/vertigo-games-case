using System.Globalization;
using Runtime.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class CurrencyView : MonoBehaviour
    {
        [Header("Currency Data")]
        [SerializeField] private CurrencyCollectibleData _currencyData;
        
        [Header("UI References")]
        [SerializeField] private Image currencyImage;
        [SerializeField] private TMP_Text currencyText;

        private bool _initialized = false;
        
        public void Initialize()
        {
            if (!_currencyData)
                return;
            if (!currencyImage) return;
            currencyImage.sprite = _currencyData.Icon;

            if (!currencyText) return; 
            
            _initialized = true;
            
            OnCurrencyChanged(_currencyData.Value);
            _currencyData.UpdateEvent += OnCurrencyChanged;
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as  RectTransform);
        }

        private void OnCurrencyChanged(int amount)
        {
            if(!_initialized)
                return;
            
            currencyText.text = amount.ToString("N0", CultureInfo.InvariantCulture);
        }

        public void Dispose()
        {
            _currencyData.UpdateEvent -= OnCurrencyChanged;
            _initialized = false;
        }
    }
}
