using System;
using System.Collections.Generic;

namespace Runtime.Data
{
    [Serializable]
    public class CollectibleSave
    {
        public string itemId;
        public int amount;
    }

    [Serializable]
    public class GameData
    {
        public int cashCount;
        public int goldCount;
        public List<CollectibleSave> collectibles = new();
    }
}
