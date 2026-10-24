using Runtime.Enums;
using UnityEngine;

namespace Runtime.Data
{
    [CreateAssetMenu(fileName = "PointCollectibleData", menuName = "Data/Collectible/PointCollectibleData")]
    public class UpgradePointCollectibleData : BaseCollectibleData
    {
        [SerializeField] private UpgradeType upgradeType;
        
        public UpgradeType UpgradeType => upgradeType;
        
        public override void GrantToPlayer(int amount,RewardSourceType source)
        {
            
        }
        
    }
}
