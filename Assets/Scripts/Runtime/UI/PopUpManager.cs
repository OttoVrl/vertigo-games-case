using System;
using System.Collections.Generic;
using Runtime.Data;
using Runtime.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    [Serializable]
    public class PopUpPairData
    {
        public Enums.PopUpState popUpState;
        public BasePopUp popUp;
    }
    
    public class PopUpManager : MonoBehaviour
    {
        [SerializeField] private Button blockerButton;
        //PanelsManager ile aynı sistem  
        [SerializeField] private List<PopUpPairData> popUpPairs = new List<PopUpPairData>();
        private Dictionary<Enums.PopUpState, BasePopUp> _popupsMap;
        
        private GlobalConfig _globalConfig;
        private BasePopUp _currentPopUp;

        public void Initialize(GlobalConfig globalConfig)
        {
            _globalConfig = globalConfig;
            
            gameObject.SetActive(true);
            
            CachePopUpsMap();
            
            foreach (var popUp in _popupsMap) popUp.Value.Initialize(globalConfig);

            StateManager.OnPopUpStateChanged -= OnPopStateUpdated;
            StateManager.OnPopUpStateChanged += OnPopStateUpdated;
        }

        private void CachePopUpsMap()
        {
            _popupsMap = new Dictionary<Enums.PopUpState, BasePopUp>();
            foreach (var popUp in popUpPairs)
            {
                if (!popUp.popUp)
                {
                    Debug.LogWarning($"Panel {popUp.popUpState} has no panel");
                }
                else
                {
                    _popupsMap.Add(popUp.popUpState, popUp.popUp);
                }
            }
        }

        private void OnPopStateUpdated(Enums.PopUpState state)
        {
            if (state == Enums.PopUpState.None)
            {
                _currentPopUp = null;
                blockerButton.gameObject.SetActive(false);
                return;
            }
            
            _popupsMap.TryGetValue(state, out var popUp);
            if (popUp == null) return;

            CloseAll();
        }

        private void BlockerButtonClicked()
        {
            if (_currentPopUp == null) return;
            
            blockerButton.gameObject.SetActive(false);

            _currentPopUp.Hide();
            _currentPopUp = null;
        }
        
        private void CloseAll()
        {
            foreach (var popUp in _popupsMap)popUp.Value.CloseWithoutAnim();
            blockerButton.gameObject.SetActive(false);
        }

        public void Dispose()
        {
            if(_globalConfig) StateManager.OnPopUpStateChanged -= OnPopStateUpdated;
            blockerButton.onClick.RemoveListener(BlockerButtonClicked);
        }
    }
}
