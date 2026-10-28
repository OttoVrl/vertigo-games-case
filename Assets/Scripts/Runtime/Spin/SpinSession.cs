using System;
using Runtime.Data;
using UnityEngine;

namespace Runtime.Spin
{
    public static class SpinSession
    {
        public static event Action OnReviveRequested;

        public static int StartZone { get; private set; }
        public static int ReviveZone { get; private set; } = 1;

        public static void Select(CustomStartType startType)
        {
            StartZone = startType != null ? Mathf.Max(1, startType.StartZoneIndex) : 1;
        }

        public static void SetReviveZone(int zone)
        {
            ReviveZone = Mathf.Max(1, zone);
        }

        public static void RequestRevive()
        {
            OnReviveRequested?.Invoke();
        }

        public static void Clear()
        {
            StartZone = 1;
            ReviveZone = 1;
        }
    }
}
