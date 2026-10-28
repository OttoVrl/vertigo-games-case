using System;
using System.Collections.Generic;
using Runtime.Managers;

namespace Runtime.Data
{
    public class DataSaver
    {
        private const string MoneyId = "currency_money";
        private const string CoinId = "currency_coin";

        private static readonly Dictionary<string, int> Amounts = new(StringComparer.Ordinal);
        private static GameData _gameData;
        private static GlobalConfig _globalConfig;

        public static void Initialize(GlobalConfig config)
        {
            _globalConfig = config;
            LoadGameData();
        }

        private static void LoadGameData()
        {
            Amounts.Clear();

            try
            {
                _gameData = SaveManager.LoadData<GameData>("game_data.dat");
            }
            catch (Exception)
            {
                _gameData = new GameData
                {
                    cashCount = 5000,
                    goldCount = 5000
                };
            }

            if (_gameData.collectibles == null)
                _gameData.collectibles = new List<CollectibleSave>();

            bool hasList = false;
            for (int i = 0; i < _gameData.collectibles.Count; i++)
            {
                CollectibleSave entry = _gameData.collectibles[i];
                if (entry == null || string.IsNullOrEmpty(entry.itemId)) continue;

                Amounts[entry.itemId] = entry.amount;
                hasList = true;
            }

            if (!hasList)
            {
                Amounts[MoneyId] = _gameData.cashCount;
                Amounts[CoinId] = _gameData.goldCount;
            }

            foreach (BaseCollectibleData collectible in _globalConfig.DatabaseConfig.AllCollectibles())
            {
                if (!collectible || string.IsNullOrEmpty(collectible.ItemId)) continue;

                if (Amounts.TryGetValue(collectible.ItemId, out int amount))
                    collectible.Value = amount;
                else
                    Amounts[collectible.ItemId] = collectible.Value;
            }

            Save();
        }

        private static void Save()
        {
            if (_gameData == null) return;

            _gameData.collectibles ??= new List<CollectibleSave>();
            _gameData.collectibles.Clear();

            foreach (KeyValuePair<string, int> pair in Amounts)
            {
                _gameData.collectibles.Add(new CollectibleSave
                {
                    itemId = pair.Key,
                    amount = pair.Value
                });
            }

            if (Amounts.TryGetValue(MoneyId, out int cash))
                _gameData.cashCount = cash;
            if (Amounts.TryGetValue(CoinId, out int gold))
                _gameData.goldCount = gold;

            SaveManager.SaveData(_gameData, "game_data.dat");
        }

        public static void SaveCollectible(string itemId, int amount)
        {
            if (string.IsNullOrEmpty(itemId)) return;

            Amounts[itemId] = amount;
            Save();
        }

        public static int GetGoldCount()
        {
            return Amounts.TryGetValue(CoinId, out int amount) ? amount : 0;
        }

        public static void AddGold(int count) => Change(CoinId, count);
        public static void AddCash(int count) => Change(MoneyId, count);
        public static void SpendGold(int count) => Change(CoinId, -count);
        public static void SpendCash(int count) => Change(MoneyId, -count);

        private static void Change(string itemId, int delta)
        {
            Amounts.TryGetValue(itemId, out int current);
            int next = current + delta;
            Amounts[itemId] = next;

            if (_globalConfig.DatabaseConfig.TryGetCollectible(itemId, out BaseCollectibleData collectible))
                collectible.Value = next;
            else
                UnityEngine.Debug.LogWarning($"{itemId} bulunamadı.");

            Save();
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/Vertigo/Delete Save")]
        private static void DeleteSaveFile()
        {
            string path = System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "game_data.dat");
            if (!System.IO.File.Exists(path))
            {
                UnityEngine.Debug.LogWarning($"Save dosyası yok: {path}");
                return;
            }

            System.IO.File.Delete(path);
            UnityEngine.Debug.Log($"Save silindi: {path}");
        }
#endif
    }
}
