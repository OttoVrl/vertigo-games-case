using Runtime.Enums;
using UnityEngine;

namespace Runtime.Data
{
    [CreateAssetMenu(fileName = "CurrencyCollectibleData", menuName = "Data/Collectible/CurrencyCollectibleData")]
    public class CurrencyCollectibleData : BaseCollectibleData
    {
        public override void GrantToPlayer(int amount, RewardSourceType rewardSource)
        {
            Value += amount;
            DataSaver.SaveCollectible(ItemId, Value);
        }
    }
}
