using Runtime.Data;

namespace Runtime.Spin
{
    public readonly struct ZoneStep
    {
        public readonly int Index;
        public readonly ZoneData Data;
        public readonly bool IsCurrent;
        
        public ZoneStep(int index, ZoneData data, bool isCurrent)
        {
            Index = index; 
            Data = data;
            IsCurrent = isCurrent;
        }
    }
}