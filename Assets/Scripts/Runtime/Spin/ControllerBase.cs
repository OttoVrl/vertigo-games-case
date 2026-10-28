using Runtime.Data;

namespace Runtime.Spin
{
    public class ControllerBase
    {
        protected readonly SpinRunState _runState;
        protected readonly SpinDatabaseConfig _config;

        public ControllerBase(SpinRunState runState, SpinDatabaseConfig config)
        {
            _runState = runState;
            _config = config;
        }
    }
}