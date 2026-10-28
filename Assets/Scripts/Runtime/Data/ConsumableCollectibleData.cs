using Runtime.Enums;
using UnityEngine;

namespace Runtime.Data
{
    [CreateAssetMenu(fileName = "ConsumableCollectibleData", menuName = "Data/Collectible/ConsumableCollectibleData")]
    public class ConsumableCollectibleData : BaseCollectibleData
    {
        public override void GrantToPlayer(int amount, RewardSourceType rewardSource)
        {
            Value += amount;
            DataSaver.SaveCollectible(ItemId, Value);
        }
    }
}