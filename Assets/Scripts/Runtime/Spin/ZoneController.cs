using System.Collections.Generic;
using Runtime.Data;

namespace Runtime.Spin
{
    public class ZoneController : ControllerBase
    {
        public int Index => _runState.Zone;
        public ZoneData Current => _config.GetZoneData(_runState.Zone);
        
        public bool IsSafe => Current && Current.IsSafe;
        public bool IsFinished => _runState.Zone > _config.ZoneStepCount;
        
        public ZoneController(SpinRunState runState, SpinDatabaseConfig config) : base(runState, config) { }

        public List<ZoneStep> GetSteps()
        {
            var steps = new List<ZoneStep>();

            for (int i = 1; i <= _config.ZoneStepCount; i++)
            {
                steps.Add(new ZoneStep(i,_config.GetZoneData(i),i==_runState.Zone));
            }
            
            return steps;
        }
        
    }
}