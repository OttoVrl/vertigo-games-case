using Runtime.Enums;
using UnityEngine;

namespace Runtime.Data
{
    public abstract class BaseCollectibleData : IntReference
    {
        [SerializeField] private string itemId;
        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;
        
        public string ItemId => itemId;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        
        public abstract void GrantToPlayer(int amount, RewardSourceType rewardSource);
    }
}
