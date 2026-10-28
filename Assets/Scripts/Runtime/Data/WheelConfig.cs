using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Data
{
    [Serializable]
    public class WheelSliceData
    {
        [SerializeField] private bool isBomb;
        [SerializeField] private RewardPool rewardPool;
        [SerializeField, Min(0)] private int weight = 10;
        public bool IsBomb => isBomb;
        public RewardPool RewardPool => rewardPool;
        public int Weight => weight;
    }
    
    [CreateAssetMenu(fileName = "WheelConfig", menuName = "Data/SpinMode/WheelConfig")]
    public class WheelConfig : ScriptableObject
    {
        [SerializeField] private Sprite wheelSprite;
        [SerializeField] private Sprite wheelIndicatorSprite;
        [SerializeField] private List<WheelSliceData> slices = new List<WheelSliceData>();
        [SerializeField] private bool hasBomb;
        
        public Sprite WheelSprite => wheelSprite;
        public Sprite WheelIndicatorSprite => wheelIndicatorSprite;
        public IReadOnlyList<WheelSliceData> Slices => slices;

        public bool HasBomb
        {
            get
            {
                foreach(var slice in slices)
                    if (slice!= null && slice.IsBomb) return true;
                return false;
            }
        }

        private void OnValidate()
        {
            int bombCount = 0;
            foreach (var slice in slices)
            {
                if(slice == null) continue;

                if (slice.IsBomb) bombCount++;
                else if (!slice.RewardPool) Debug.Log("slice has no reward pool");
            }
        }
    }
}