using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Data
{
    [Serializable]
    public class SpinRewardEntry
    {
        [SerializeField] private BaseCollectibleData collectibleData;
        [SerializeField] private int minAmount;
        [SerializeField] private int maxAmount;
        [SerializeField] private int weight;
        [SerializeField,Min(1)] private int unlockZone;
        
        
        
        public BaseCollectibleData CollectibleData => collectibleData;
        public int MinAmount => minAmount;
        public int MaxAmount => maxAmount;
        public int Weight => weight;
        public int UnlockZone => unlockZone;
        
    }
    
    [CreateAssetMenu(fileName = "RewardPool", menuName = "Data/Spin/RewardPool")]
    public class RewardPool : ScriptableObject
    {
        [SerializeField] private List<SpinRewardEntry> entries = new List<SpinRewardEntry>();

        public bool TryPick(int zone,ICollection<BaseCollectibleData> excluded,out SpinRewardEntry resultEntry)
        {
            resultEntry = null;
            
            int totalWeight = 0;

            foreach (var entry in entries)
                if(IsAvailable(entry,zone,excluded)) totalWeight += entry.Weight;

            if (totalWeight == 0) return false;
            
            int roll = UnityEngine.Random.Range(0, totalWeight);

            foreach (var entry in entries)
            {
                if(!IsAvailable(entry,zone,excluded)) continue;
                
                roll -= entry.Weight;
                if (roll < 0)
                {
                    resultEntry = entry;
                    return true;
                }
            }
                
            return false;
        }

        private bool IsAvailable(SpinRewardEntry entry, int zone, ICollection<BaseCollectibleData> excluded)
        {
            return entry != null
                && entry.CollectibleData
                   && entry.Weight>0
                   && zone > entry.UnlockZone
                   && !excluded.Contains(entry.CollectibleData);
        }
    }
}
