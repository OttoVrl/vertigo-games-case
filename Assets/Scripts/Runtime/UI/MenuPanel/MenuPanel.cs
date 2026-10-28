using Runtime.Data;
using Runtime.Enums;
using Runtime.Extensions;
using Runtime.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class MenuPanel : BasePanel
    {
        [SerializeField] private Button spinModeButton;

        public override void Initialize(GlobalConfig globalConfig)
        {
            base.Initialize(globalConfig);
            
            if(spinModeButton != null)
                spinModeButton.onClick.AddListener(_OnClickSpinModeButton);
        }

        private void _OnClickSpinModeButton()
        {
            StateManager.SetPopUpState(PopUpState.SpinModeEntryPopUp);
        }

        public override void Dispose()
        {
            base.Dispose();
            if(spinModeButton != null)
                spinModeButton.onClick.RemoveListener(_OnClickSpinModeButton);
        }

        private void OnValidate()
        {
            if (!spinModeButton)
                spinModeButton = this.FindChild<Button>("ui_spin_mode_button");
        }
    }
}
