using System.Collections.Generic;
using Runtime.Data;
using Runtime.Enums;
using Runtime.Extensions;
using Runtime.Managers;
using Runtime.Spin;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class RewardsResultPanel : BasePanel
    {
        [SerializeField] private SpinRewardCardView _rewardCardViewPrefab;
        [SerializeField] private RectTransform _rewardCardsContainer;
        [SerializeField] private Button continueButton;

        private readonly List<SpinRewardCardView> _cards = new();
        private bool _presented;

        public override void Initialize(GlobalConfig globalConfig)
        {
            base.Initialize(globalConfig);
        }

        public override void Show()
        {
            if (!ClaimedRewards.HasAny)
            {
                StateManager.SetGameState(GameState.Menu);
                return;
            }

            base.Show();
            continueButton.onClick.AddListener(_OnClickContinue);
            BuildCards();
            _presented = true;
        }

        private void BuildCards()
        {
            ClearCards();
            foreach (ClaimedReward reward in ClaimedRewards.Rewards)
            {
                SpinRewardCardView card = Factory.Create(_rewardCardViewPrefab, _rewardCardsContainer);
                card.SetReward(reward.Data, reward.Amount);
                _cards.Add(card);
            }
        }

        private void _OnClickContinue()
        {
            StateManager.SetGameState(GameState.Menu);
        }

        public override void Hide()
        {
            ClearCards();
            if (_presented)
                ClaimedRewards.Clear();
            _presented = false;
            base.Hide();
            continueButton.onClick.RemoveAllListeners();
        }

        private void ClearCards()
        {
            for (int i = 0; i < _cards.Count; i++)
            {
                if (_cards[i])
                    Factory.Destroy(_cards[i].gameObject);
            }

            _cards.Clear();
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        private void OnValidate()
        {
            if (!continueButton)
                continueButton = this.FindChild<Button>("ui_continue_button");
        }
    }
}
