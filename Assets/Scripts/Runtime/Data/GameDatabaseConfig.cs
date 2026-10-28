using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.Data
{
    [CreateAssetMenu(fileName = "DatabaseConfig", menuName = "Data/Config/DatabaseConfig")]
    public class GameDatabaseConfig : ScriptableObject
    {
        private static readonly IReadOnlyList<CurrencyCollectibleData> EmptyCurrencies = new List<CurrencyCollectibleData>();
        private static readonly IReadOnlyList<UnlockPointCollectibleData> EmptyUnlockPoints = new List<UnlockPointCollectibleData>();
        private static readonly IReadOnlyList<UpgradePointCollectibleData> EmptyUpgradePoints = new List<UpgradePointCollectibleData>();
        private static readonly IReadOnlyList<ChestPointCollectibleData> EmptyChestPoints = new List<ChestPointCollectibleData>();
        private static readonly IReadOnlyList<ConsumableCollectibleData> EmptyConsumables = new List<ConsumableCollectibleData>();

        [SerializeField] private bool autoTrackCollectibles;
        [SerializeField] private List<CurrencyCollectibleData> currencies = new List<CurrencyCollectibleData>();
        [SerializeField] private List<UnlockPointCollectibleData> unlockPoints = new List<UnlockPointCollectibleData>();
        [SerializeField] private List<UpgradePointCollectibleData> upgradePoints = new List<UpgradePointCollectibleData>();
        [SerializeField] private List<ChestPointCollectibleData> chestPoints = new List<ChestPointCollectibleData>();
        [SerializeField, FormerlySerializedAs("items")] private List<ConsumableCollectibleData> consumables = new List<ConsumableCollectibleData>();

        public bool AutoTrackCollectibles => autoTrackCollectibles;
        public IReadOnlyList<CurrencyCollectibleData> Currencies => currencies ?? EmptyCurrencies;
        public IReadOnlyList<UnlockPointCollectibleData> UnlockPoints => unlockPoints ?? EmptyUnlockPoints;
        public IReadOnlyList<UpgradePointCollectibleData> UpgradePoints => upgradePoints ?? EmptyUpgradePoints;
        public IReadOnlyList<ChestPointCollectibleData> ChestPoints => chestPoints ?? EmptyChestPoints;
        public IReadOnlyList<ConsumableCollectibleData> Consumables => consumables ?? EmptyConsumables;

        [NonSerialized] private Dictionary<string, BaseCollectibleData> _collectiblesById;

        public IEnumerable<BaseCollectibleData> AllCollectibles()
        {
            EnsureCache();
            return _collectiblesById.Values;
        }

        public bool TryGetCollectible(string itemId, out BaseCollectibleData collectible)
        {
            collectible = null;
            if (string.IsNullOrEmpty(itemId))
                return false;

            EnsureCache();
            return _collectiblesById.TryGetValue(itemId, out collectible);
        }

        public bool TryGetCollectible<T>(string itemId, out T collectible) where T : BaseCollectibleData
        {
            if (TryGetCollectible(itemId, out BaseCollectibleData found) && found is T typed)
            {
                collectible = typed;
                return true;
            }

            collectible = null;
            return false;
        }

        private void OnEnable()
        {
            _collectiblesById = null;
        }

        private void OnValidate()
        {
            currencies ??= new List<CurrencyCollectibleData>();
            unlockPoints ??= new List<UnlockPointCollectibleData>();
            upgradePoints ??= new List<UpgradePointCollectibleData>();
            chestPoints ??= new List<ChestPointCollectibleData>();
            consumables ??= new List<ConsumableCollectibleData>();
            _collectiblesById = null;
        }

        private void EnsureCache()
        {
            if (_collectiblesById != null)
                return;

            _collectiblesById = new Dictionary<string, BaseCollectibleData>(StringComparer.Ordinal);
            AddToCache(currencies);
            AddToCache(unlockPoints);
            AddToCache(upgradePoints);
            AddToCache(chestPoints);
            AddToCache(consumables);
        }

        private void AddToCache<T>(List<T> source) where T : BaseCollectibleData
        {
            if (source == null)
                return;

            for (int i = 0; i < source.Count; i++)
            {
                T collectible = source[i];
                if (collectible == null)
                    continue;

                string itemId = collectible.ItemId;
                if (string.IsNullOrEmpty(itemId))
                {
                    Debug.LogWarning($"[GameDatabaseConfig] '{collectible.name}' has no itemId and was skipped.", collectible);
                    continue;
                }

                if (!_collectiblesById.TryAdd(itemId, collectible))
                    Debug.LogWarning($"[GameDatabaseConfig] Duplicate itemId '{itemId}' on '{collectible.name}'. Keeping the first entry.", collectible);
            }
        }
    }
}
