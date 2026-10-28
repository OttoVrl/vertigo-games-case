using Runtime.Data;
using TMPro;
using UnityEngine;

namespace Runtime.UI
{
    public class SafeZoneInfoView : MonoBehaviour
    {
        [SerializeField] private ZoneData zoneData;
        [SerializeField] private TMP_Text indexText;

        public void Refresh(int currentZone, SpinDatabaseConfig config)
        {
            int nextIndex = FindNext(currentZone, config);
            if (nextIndex < 0)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);
            if (indexText)
                indexText.text = nextIndex.ToString();
        }

        private int FindNext(int currentZone, SpinDatabaseConfig config)
        {
            if (!zoneData || !config) return -1;

            for (int i = currentZone + 1; i <= config.ZoneStepCount; i++)
            {
                if (config.GetZoneData(i) == zoneData)
                    return i;
            }

            return -1;
        }
    }
}
