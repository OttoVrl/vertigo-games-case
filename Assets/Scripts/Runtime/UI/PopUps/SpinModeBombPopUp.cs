using Runtime.Data;
using Runtime.Enums;
using Runtime.Extensions;
using Runtime.Managers;
using Runtime.Spin;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class SpinModeBombPopUp : BasePopUp
    {
        [SerializeField] private RectTransform cardRectTransform;
        [SerializeField] private CostlyButton reviveButton;

        private int _zone = 1;

        public override void Initialize(GlobalConfig globalConfig)
        {
            base.Initialize(globalConfig);

            if (reviveButton)
                reviveButton.OnClickSuccess += _OnClickRevive;
        }

        private void _OnClickRevive()
        {
            SpinDatabaseConfig config = _globalConfig.SpinDatabaseConfig;
            CurrencyCollectibleData currency = config.ReviveCostCurrencyType;
            int cost = config.GetReviveCost(_zone);
            if (!currency) return;

            if (currency.ItemId == "currency_money")
                DataSaver.SpendCash(cost);
            else
                DataSaver.SpendGold(cost);
            
            SpinSession.RequestRevive();
            CloseWithoutAnim();
            StateManager.SetPopUpState(PopUpState.None);
        }

        public override void Show()
        {
            base.Show();
            SfxManager.Instance.PlaySFX(SFXType.SpinModeBomb);
            _zone = SpinSession.ReviveZone;

            SpinDatabaseConfig config = _globalConfig.SpinDatabaseConfig;
            CurrencyCollectibleData currency = config.ReviveCostCurrencyType;
            if (!reviveButton || !currency) return;

            reviveButton.Initialize(config.GetReviveCost(_zone), currency);
        }

        public override void Hide()
        {
            base.Hide();
            StateManager.SetGameState(GameState.Menu);
        }

        private void OnValidate()
        {
            if (!closeButton)
                closeButton = this.FindChild<Button>("ui_giveup_button");
        }
    }
}
