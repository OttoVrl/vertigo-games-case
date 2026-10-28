using Runtime.Data;
using Runtime.Enums;
using Runtime.Managers;
using Runtime.Spin;
using UnityEngine;

namespace Runtime.UI
{
    public class SpinModeEntryPopUp : BasePopUp
    {
        [SerializeField] private CostlyButton normalPlayButton;
        [SerializeField] private CostlyButton skipStepsButton;
            
        public override void Initialize(GlobalConfig globalConfig)
        {
            base.Initialize(globalConfig);
            
            CustomStartType normalStart = globalConfig.SpinDatabaseConfig.CustomStartTypes[0];
            CustomStartType skipStepsStart = globalConfig.SpinDatabaseConfig.CustomStartTypes[1];
            
            normalPlayButton.Initialize(normalStart.Cost,normalStart.TargetCurrencyData);
            skipStepsButton.Initialize(skipStepsStart.Cost,skipStepsStart.TargetCurrencyData);

            normalPlayButton.OnClickSuccess += () => _Begin(normalStart);
            skipStepsButton.OnClickSuccess += () => _Begin(skipStepsStart);
            
        }

        private void _Begin(CustomStartType startType)
        {
            DataSaver.SpendGold(startType.Cost);
            
            SpinSession.Select(startType);
            StateManager.SetPopUpState(PopUpState.None);
            StateManager.SetGameState(GameState.SpinGame);
        }
    }
}
