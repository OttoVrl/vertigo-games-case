using System.Collections.Generic;
using Runtime.Data;

namespace Runtime.Spin
{
    public class WheelRoller
    {
        private readonly HashSet<BaseCollectibleData> _usedCollectibles = new HashSet<BaseCollectibleData>();
        private readonly HashSet<BaseCollectibleData> _noExclusion = new  HashSet<BaseCollectibleData>();

        public bool TryToRoll(WheelConfig wheel, int zone, float rewardMultiplier, List<RolledSlice> result)
        {
            result.Clear();
            _usedCollectibles.Clear();
            //
            if(!wheel || wheel.Slices.Count == 0)
                return false;

            foreach (var slice in wheel.Slices)
            {
                if (slice.IsBomb)
                {
                    result.Add(new RolledSlice(null, 0, slice.Weight, true));
                    continue;
                }

                if (!slice.RewardPool) 
                    return false;
               
                //Önce farklı ödül denedim havyuz yuetmezse tekrara izin verdim(mesela iki cash dilimi oluşması gibi)  
                if(!slice.RewardPool.TryPick(zone,_usedCollectibles,out var entry) && !slice.RewardPool.TryPick(zone,_noExclusion,out entry))
                    return false;
                
                _usedCollectibles.Add(entry.CollectibleData);
                
                int amount = entry.RollAmount(rewardMultiplier);
                
                result.Add(new RolledSlice(entry.CollectibleData, amount,slice.Weight,false));
            }

            return true;
        }
    }
}
