using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Spin
{
    public static class WeightedRandom
    {
        public static int PickIndex(IReadOnlyList<RolledSlice> slices)
        {
            int totalWeight = 0;

            for (int i = 0; i < slices.Count; i++)
            {
                if (slices[i].IsClosed || slices[i].Weight <= 0) continue;
                totalWeight += slices[i].Weight;
            }
            
            if(totalWeight<=0) 
                return -1;
            
            int roll = Random.Range(0, totalWeight);
            
            for (int i = 0; i < slices.Count; i++)
            {
                if (slices[i].IsClosed || slices[i].Weight <= 0) continue;

                roll -= slices[i].Weight;
                if (roll < 0)
                    return i;
            }

            return -1;
        }
    }
}
