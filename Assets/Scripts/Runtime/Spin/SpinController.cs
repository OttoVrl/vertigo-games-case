using System.Collections.Generic;
using Runtime.Data;
using Runtime.Enums;

namespace Runtime.Spin
{
    public class SpinController : ControllerBase
    {
        private readonly WheelRoller _wheelRoller = new();
        private readonly List<RolledSlice> _slices = new();
        
        public bool IsSpinning { get; private set; }
        public IReadOnlyList<RolledSlice> Slices => _slices;
        
        public bool CanSpin => !IsSpinning && !_runState.IsLost && _slices.Count > 0;
        
        public SpinController(SpinRunState runState, SpinDatabaseConfig config) : base(runState, config) { }

        public bool Prepare(ZoneData zoneData)
        {
            _slices.Clear();
            if (!zoneData) return false;
            
            return _wheelRoller.TryToRoll(zoneData.WheelConfig,_runState.Zone,_config.GetRewardMultiplier(_runState.Zone),_slices);
        }

        public bool TryPick(out int index,out RolledSlice slice)
        {
            index = -1;
            slice = default;
            
            if(!CanSpin) return false;

            index = WeightedRandom.PickIndex(_slices);
            
            if (index < 0) return false;

            slice = _slices[index];
            IsSpinning = true;
            
            return true;
        }

        public SpinResultType Complete(RolledSlice slice)
        {
            IsSpinning = false;
            if (slice.IsBomb)
            {
                _runState.Lose();
                return SpinResultType.Bomb;
            }
            
            _runState.AddReward(slice.collectibleData,slice.Amount);
            _runState.NextZone();
            
            return SpinResultType.Reward;
        }

        public int Revive()
        {
            int bombIndex = _slices.FindIndex(slice => slice.IsBomb && !slice.IsClosed);
            if (bombIndex >= 0)
                _slices[bombIndex] = _slices[bombIndex].Closed();

            IsSpinning = false;
            _runState.Revive();
            return bombIndex;
        }
    }
}