using Runtime.Enums;
using UnityEngine;

namespace Runtime.Data
{
    [CreateAssetMenu(fileName = "ChestPointCollectibleData", menuName = "Data/Collectible/ChestPointCollectibleData")]
    public class ChestPointCollectibleData : BaseCollectibleData
    {
        [SerializeField] private ChestType chestType;
        
        public ChestType ChestType => chestType;
        
        public override void GrantToPlayer(int amount, RewardSourceType rewardSource)
        {
                 
        }
    }
}
