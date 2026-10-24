using System;
using UnityEngine;

namespace Runtime.Data
{
    [CreateAssetMenu(fileName = "SafeZoneData", menuName = "Data/SpinMode/SafeZoneData")]
    public class ZoneData : ScriptableObject
    {
        [SerializeField] private string zoneKey;
        [SerializeField] private string displayName;
        [SerializeField,Min(1)] private int stepInterval;
        [SerializeField] private Color zoneColor;
        
        [SerializeField] private WheelConfig wheelConfig;

        
        public string ZoneKey => zoneKey;
        public string DisplayName => displayName;
        public int StepInterval => stepInterval;
        public Color ZoneColor => zoneColor;
        
        public WheelConfig WheelConfig => wheelConfig;

        public bool IsSafe => wheelConfig && !wheelConfig.HasBomb;
        
        public bool Matches(int zone) => zone % stepInterval == 0;

    }

    
}