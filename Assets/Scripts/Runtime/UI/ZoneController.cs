using Runtime.Data;
using UnityEngine;

namespace Runtime.UI
{
    public class ZoneController : MonoBehaviour
    {
        [SerializeField] private ZoneElement zoneElementPrefab;
        
        private const int StartStepIndex = 7;
        
        private GlobalConfig _globalConfig;
        
        public void Initialize(GlobalConfig globalConfig)
        {
            _globalConfig = globalConfig;
        }

        private void InitializeZoneArea()
        {
            
        }

        private void NextStep()
        {
            
        }

        private void Show()
        {
            
        }

        private void Hide()
        {
            
        }
    }
}
