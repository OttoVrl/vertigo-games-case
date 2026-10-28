using System;
using System.Collections.Generic;
using Runtime.Data;
using Runtime.Spin;
using UnityEngine;

namespace Runtime.UI
{
    public class SpinView : MonoBehaviour
    {
        [SerializeField] private WheelView wheelView;

        public bool IsChanging => wheelView.IsChanging;

        public void Show(ZoneData zoneData, IReadOnlyList<RolledSlice> slices, Action onReady = null)
        {
            WheelConfig wheel = zoneData ? zoneData.WheelConfig : null;
            wheelView.Initialize(
                wheel ? wheel.WheelSprite : null,
                wheel ? wheel.WheelIndicatorSprite : null,
                slices,
                onReady);
        }

        public void Play(int sliceIndex, int sliceCount, Action onComplete)
        {
            wheelView.SpinTo(sliceIndex,sliceCount, onComplete);
        }


        public void Skip()
        {
            wheelView.Skip();
        }

        public void CloseSlice(int sliceIndex)
        {
            wheelView.CloseSlice(sliceIndex);
        }

        public void Stop()
        {
            wheelView.Stop();
        }
        
    }
}
