using Runtime.Enums;
using Runtime.Extensions;
using Runtime.Managers;
using Runtime.Spin;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class SpinModeCompletePopUp : BasePopUp
    {
        [SerializeField] private Button claimButton;
        
        public override void Show()
        {
            base.Show();
            claimButton.onClick.AddListener(_OnClickClaim);
        }

        private void _OnClickClaim()
        {
            ClaimedRewards.RequestClaim();
            StateManager.SetPopUpState(PopUpState.None);
            StateManager.SetGameState(GameState.RewardsResult);
        }


        public override void Hide()
        {
            base.Hide();
            claimButton.onClick.RemoveListener(_OnClickClaim);
        }

        private void OnValidate()
        {
            if (!claimButton)
                claimButton = this.FindChild<Button>("ui_claim_button");
        }
        
    }
}