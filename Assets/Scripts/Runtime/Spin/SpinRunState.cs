using System.Collections.Generic;
using Runtime.Data;
using Runtime.Enums;
using UnityEngine;

namespace Runtime.Spin
{
    public class SpinRunState
    {
        private readonly Dictionary<BaseCollectibleData,int> _pendingRewards = new Dictionary<BaseCollectibleData,int>();

        public int Zone {get; private set; } = 0;
        public bool IsLost { get; private set; }
        
        public IReadOnlyDictionary<BaseCollectibleData,int> PendingRewards => _pendingRewards;

        public void AddReward(BaseCollectibleData collectible, int amount)
        {
            _pendingRewards.TryGetValue(collectible, out int curent);
            _pendingRewards[collectible] = curent + amount;
        }
        
        //Burayı ZoneController ile bağlıcam
        public void NextZone() => Zone++;

        public void Lose()
        {
            IsLost = true;
        }

        public void Revive()
        {
            IsLost = false;
        }

        public void ClaimAll(RewardSourceType source)
        {
            ClaimedRewards.Store(_pendingRewards);
            foreach (var reward in _pendingRewards)
            {
                if (!reward.Key) continue;
                reward.Key.GrantToPlayer(reward.Value, source);
            }

            Restart();
        }

        public void Restart(int startZone = 1)
        {
            _pendingRewards.Clear();
            Zone = Mathf.Max(1,startZone);
            IsLost = false;
        }
    }
}
