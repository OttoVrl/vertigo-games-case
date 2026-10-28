using Runtime.Data;
using Runtime.Enums;
using Runtime.UI;
using UnityEngine;

namespace Runtime.Managers
{
    public class UIManager : MonoBehaviour
    {
        private GlobalConfig _globalConfig;
        [SerializeField] private PanelManager panelManager;
        [SerializeField] private PopUpManager popUpManager;
        [SerializeField] private CurrencyView cashView;
        [SerializeField] private CurrencyView goldView;
        public void Initialize(GlobalConfig globalConfig)
        {
            _globalConfig = globalConfig;
            
            panelManager.Initialize(_globalConfig);
            popUpManager.Initialize(_globalConfig);
            cashView.Initialize();
            goldView.Initialize();
        }

        public void Dispose()
        {
            panelManager.Dispose();
            popUpManager.Dispose();
            cashView.Dispose();
            goldView.Dispose();
        }
    }
}
