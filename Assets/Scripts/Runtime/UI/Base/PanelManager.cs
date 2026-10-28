using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Data;
using Runtime.Managers;
using UnityEngine;

namespace Runtime.UI
{
    [Serializable]
    public class PanelPairData
    {
        public Enums.GameState gameState;
        public BasePanel panel;
    }
    public class PanelManager : MonoBehaviour
    {
        //Paneller için burda SerializedDictionary(Enum,Panel) Sistemide yazılıp PanelData iptal edilebilir.
        //ancak burda PanelData listesinden cache liyerek Dictionaryi dolduracağım ve inspector için küçük bir editor script yazacağım.
        //Scripts/Editor/UI/PanelManagerEditor.cs
        
        [SerializeField] private List<PanelPairData> panelPairs = new List<PanelPairData>();
        private Dictionary<Enums.GameState, BasePanel> _panelsMap;

        public void Initialize(GlobalConfig globalConfig)
        {
            gameObject.SetActive(true);
            
            CachePanelsMap();

            foreach (var panel in _panelsMap)
                panel.Value.Initialize(globalConfig);
            
            StateManager.OnGameStateChanged += OnGameStateChanged;
        }

        private void CachePanelsMap()
        {
            _panelsMap = new Dictionary<Enums.GameState, BasePanel>();
            foreach (var panel in panelPairs)
            {
                if (!panel.panel)
                {
                    Debug.LogWarning($"Panel {panel.gameState} has no panel");
                }
                else
                {
                    _panelsMap.Add(panel.gameState, panel.panel);
                }
            }
        }

        private void OnGameStateChanged(Enums.GameState state)
        {
            if (!_panelsMap.TryGetValue(state, out var panel)) return;
            if (panel == null) return;
            
            CloseAll();
            
            panel.Show();
        }

        private void CloseAll()
        {
            foreach (var p in _panelsMap)
            {
                p.Value.Hide();
            }
        }
        
        
        public void Dispose()
        {
            foreach (var panel in _panelsMap) panel.Value.Dispose();
            StateManager.OnGameStateChanged -= OnGameStateChanged;
        }
    }
}
