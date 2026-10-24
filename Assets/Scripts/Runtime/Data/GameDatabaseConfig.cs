using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Data
{
    [CreateAssetMenu(fileName = "DatabaseConfig", menuName = "Data/Config/DatabaseConfig")]
    public class GameDatabaseConfig : ScriptableObject
    {
        private static readonly IReadOnlyList<CurrencyCollectibleData> EmptyCurrencies = new List<CurrencyCollectibleData>();
        private static readonly IReadOnlyList<UnlockPointCollectibleData> EmptyUnlockPoints = new List<UnlockPointCollectibleData>();
        private static readonly IReadOnlyList<UpgradePointCollectibleData> EmptyUpgradePoints = new List<UpgradePointCollectibleData>();

        [SerializeField] private bool autoTrackCollectibles;
        [SerializeField] private List<CurrencyCollectibleData> currencies = new List<CurrencyCollectibleData>();
        [SerializeField] private List<UnlockPointCollectibleData> unlockPoints = new List<UnlockPointCollectibleData>();
        [SerializeField] private List<UpgradePointCollectibleData> upgradePoints = new List<UpgradePointCollectibleData>();

        public bool AutoTrackCollectibles => autoTrackCollectibles;
        public IReadOnlyList<CurrencyCollectibleData> Currencies => currencies ?? EmptyCurrencies;
        public IReadOnlyList<UnlockPointCollectibleData> UnlockPoints => unlockPoints ?? EmptyUnlockPoints;
        public IReadOnlyList<UpgradePointCollectibleData> UpgradePoints => upgradePoints ?? EmptyUpgradePoints;

        private void OnValidate()
        {
            currencies ??= new List<CurrencyCollectibleData>();
            unlockPoints ??= new List<UnlockPointCollectibleData>();
            upgradePoints ??= new List<UpgradePointCollectibleData>();
        }
    }
}
