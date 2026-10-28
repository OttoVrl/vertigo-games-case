using System;
using System.Collections.Generic;
using Runtime.Data;

namespace Runtime.Spin
{
    public readonly struct ClaimedReward
    {
        public readonly BaseCollectibleData Data;
        public readonly int Amount;

        public ClaimedReward(BaseCollectibleData data, int amount)
        {
            Data = data;
            Amount = amount;
        }
    }

    public static class ClaimedRewards
    {
        private static readonly List<ClaimedReward> RewardList = new();

        public static event Action OnClaimRequested;
        public static IReadOnlyList<ClaimedReward> Rewards => RewardList;
        public static bool HasAny => RewardList.Count > 0;

        public static void Store(IReadOnlyDictionary<BaseCollectibleData, int> pending)
        {
            RewardList.Clear();
            if (pending == null) return;

            var ordered = new List<StoredReward>();
            int inserted = 0;
            foreach (var pair in pending)
            {
                if (!pair.Key || pair.Value <= 0) continue;
                ordered.Add(new StoredReward(pair.Key, pair.Value, inserted));
                inserted++;
            }

            ordered.Sort((a, b) =>
            {
                int order = a.Data.CaseOrderIndex.CompareTo(b.Data.CaseOrderIndex);
                if (order != 0)
                    return order;
                return a.Inserted.CompareTo(b.Inserted);
            });

            for (int i = 0; i < ordered.Count; i++)
                RewardList.Add(new ClaimedReward(ordered[i].Data, ordered[i].Amount));
        }

        public static void RequestClaim()
        {
            OnClaimRequested?.Invoke();
        }

        public static void Clear()
        {
            RewardList.Clear();
        }

        private readonly struct StoredReward
        {
            public readonly BaseCollectibleData Data;
            public readonly int Amount;
            public readonly int Inserted;

            public StoredReward(BaseCollectibleData data, int amount, int inserted)
            {
                Data = data;
                Amount = amount;
                Inserted = inserted;
            }
        }
    }
}
