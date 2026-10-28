using System.Collections.Generic;
using Runtime.Data;
using Runtime.Enums;

namespace Runtime.Spin
{
    public class CaseController : ControllerBase
    {
        public IReadOnlyDictionary<BaseCollectibleData, int> PendingRewards => _runState.PendingRewards;
        public bool IsLost => _runState.IsLost;
        
        public CaseController(SpinRunState runState, SpinDatabaseConfig config) : base(runState, config) { }

        public int GetPending(BaseCollectibleData collectible)
        {
            PendingRewards.TryGetValue(collectible, out int amount);
            return amount;
        }
        
        public void Claim()
        {
            _runState.ClaimAll(RewardSourceType.SpinMode);
        }
    }
}