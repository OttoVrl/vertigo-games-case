using Runtime.Data;
using UnityEngine;

namespace Runtime.Spin
{
    public readonly struct RolledSlice
    {
        public readonly BaseCollectibleData collectibleData;
        public readonly int Amount;
        public readonly int Weight;
        public readonly bool IsBomb;
        public readonly bool IsClosed;

        public RolledSlice(BaseCollectibleData collectible, int amount, int weight, bool isBomb, bool isClosed = false)
        {
            collectibleData = collectible;
            Amount = amount;
            Weight = weight;
            IsBomb = isBomb;
            IsClosed = isClosed;
        }

        public RolledSlice Closed()
        {
            return new RolledSlice(collectibleData, Amount, 0, IsBomb, true);
        }
    }
}
