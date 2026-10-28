using Runtime.Enums;
using UnityEngine;

namespace Runtime.Data
{
    [CreateAssetMenu(fileName = "UnlockPointCollectibleData", menuName = "Data/Collectible/UnlockPointCollectibleData")]
    public class UnlockPointCollectibleData : BaseCollectibleData
    {
        public override void GrantToPlayer(int amount, RewardSourceType rewardSource)
        {
            Value += amount;
            DataSaver.SaveCollectible(ItemId, Value);
        }
    }
}
