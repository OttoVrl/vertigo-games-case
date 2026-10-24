using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime.Data
{
    
    
    [CreateAssetMenu(fileName = "SpinModeConfig", menuName = "Data/SpinMode/SpinModeConfig")]
    public class SpinDatabaseConfig : ScriptableObject
    {
        [SerializeField] private int zoneStepCount = 60;
        [SerializeField] private float rewardIncreasePerZone = 0.1f;
        
        /*Bu kısımda ZoneType adında enum tanımlanıp default wheel dahil olmak üzere 3 tane WheelConfig kullanarak kontrol ordanda sağlanabilirdi ancak daha scaleble olması,Özel Zoneların
         designer tarafından daha kolay tanımlanabilmesi ve revizeyi azaltmak adına zoneData listesi sistemi yazmaya karar verdim.
        */
        [SerializeField] private List<ZoneData> zoneDatas;

        public int ZoneStepCount => zoneStepCount;

        public ZoneData GetZoneData(int zoneIndex)
        {
            if (zoneDatas != null && zoneDatas.Count > 0)
            {
                
                foreach (var zone in zoneDatas)
                {
                    if(zone && zone.Matches(zoneIndex)) return zone;
                }
            }
            
            return null;
        }

        public float GetRewardMultiplier(int zoneIndex)
        {
            return 1f + (zoneIndex-1) * rewardIncreasePerZone;
        }
        
        private int GetInterval(ZoneData zone) => zone ? zone.StepInterval : 0;

        public void OnValidate()
        {
            //girilen  zone stepIntervalleri kendinden küçük zone'un katı olması ihtimaline karşın büyükden küçüğe tekrar sıraladım(örnk; verilen case dökümanda 30 aynı zamanda 5 katı. Büyük olana önce bakılmalı)
            //Designer direk listeyi büyükten küçüğede dizebilirdi ancak güvenli yapmak istedim
            if (zoneDatas != null && zoneDatas.Count > 0)
            {
                zoneDatas.Sort((a,b)=>GetInterval(b).CompareTo(GetInterval(a)));
            }
            
        }

    }
}
