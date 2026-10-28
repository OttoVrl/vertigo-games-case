using System;
using System.Collections.Generic;
using DG.Tweening;
using Runtime.Data;
using Runtime.Enums;
using Runtime.Extensions;
using Runtime.Managers;
using Runtime.Spin;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Runtime.UI
{
    [Serializable]
    public class BackgroundByZone
    {
        [SerializeField] private ZoneData zoneData;
        [SerializeField] private Color backgroundColor;
        
        public ZoneData ZoneData => zoneData;
        public Color BackgroundColor => backgroundColor;
    }
    public class SpinPanel : BasePanel
    {
        private SpinDatabaseConfig _spinConfig;

        [Header("Background Settings")]
        [SerializeField] private Image backgroundImage;
        [SerializeField] private List<BackgroundByZone> backgroundByZones;

        [Header("Views")]
        [SerializeField] private SpinView spinView;
        [SerializeField] private ZoneView zoneView;
        [SerializeField] private CaseView caseView;
        [SerializeField] private SpinRewardCardView spinRewardCardView;
        [SerializeField] private List<SafeZoneInfoView> safeZoneInfoViews;

        [Header("UI References")]
        [SerializeField] private Button spinButton;
        [SerializeField] private Button skipButton;

        private SpinRunState _runState;
        private SpinController _spinController;
        private ZoneController _zoneController;
        private CaseController _caseController;

        private bool CanExit => _zoneController.IsSafe && !_spinController.IsSpinning && !_caseController.IsLost;
        
        
        public override void Initialize(GlobalConfig globalConfig)
        {
            base.Initialize(globalConfig);
            
            SpinDatabaseConfig spinConfig = globalConfig.SpinDatabaseConfig;
            _spinConfig = spinConfig;
            
            _runState = new SpinRunState();
            _spinController = new SpinController(_runState, spinConfig);
            _zoneController = new ZoneController(_runState, spinConfig);
            _caseController = new CaseController(_runState, spinConfig);

            caseView.Initialize(Leave);
            
            spinButton?.onClick.AddListener(_Spin);
            skipButton?.onClick.AddListener(_Skip);
            
        }


        private void StartRun()
        {
            _runState.Restart(SpinSession.StartZone);
            SpinSession.Clear();
            zoneView.Show(_zoneController.GetSteps(),_zoneController.Index);
            RefreshZone();
            RefreshButtons();
        }

        private void _Spin()
        {
            if (_spinController.IsSpinning)
            {
                _Skip();
                return;
            }

            if (spinView.IsChanging) return;
            if (!_spinController.TryPick(out int index, out RolledSlice slice)) return;
            
            RefreshButtons();
            spinView.Play(index,_spinController.Slices.Count,()=> SpinFinished(slice));
        }

        private void _Skip()
        {
            if (!_spinController.IsSpinning) return;
            spinView.Skip();
        }

        private void SpinFinished(RolledSlice slice)
        {
            SpinResultType result = _spinController.Complete(slice);
            if (result == SpinResultType.Bomb)
            {
                SpinSession.SetReviveZone(_zoneController.Index);
                StateManager.SetPopUpState(PopUpState.SpinModeBombPopUp);
                return;
            }

            var element = caseView.ShowReward(slice.collectibleData);
            int targetAmount = _caseController.GetPending(slice.collectibleData);
            
            bool showResult = spinRewardCardView.TryShow(slice.collectibleData, slice.Amount, () =>
            {
                FlyingRewardPlayer.Instance.Play(
                    slice.collectibleData.Icon,
                    slice.Amount,
                    spinRewardCardView.transform as RectTransform,
                    element.GetIconRect(),
                    () => caseView.SetRewardAmount(slice.collectibleData, targetAmount));
                
            }, AdvanceAfterReward);

            if (!showResult)
                AdvanceAfterReward();
        }

        private void AdvanceAfterReward()
        {
            if (_zoneController.IsFinished)
            {
                StateManager.SetPopUpState(PopUpState.SpinModeCompletePopUp);
                return;
            }

            zoneView.NextStep();
            RefreshZone();
            RefreshButtons();
        }

        private void Leave()
        {
            if (!CanExit) return;
            StateManager.SetPopUpState(PopUpState.SpinModeExitPopUp);
        }

        private void OnClaimRequested()
        {
            _caseController.Claim();
        }

        private void RefreshZone()
        {
            ZoneData zoneData = _zoneController.Current;
            if(_spinController.Prepare(zoneData))
                spinView.Show(zoneData, _spinController.Slices, RefreshButtons);
            ApplyBackground(zoneData);
            RefreshSafeZoneInfo();
        }

        private void RefreshSafeZoneInfo()
        {
            if (safeZoneInfoViews == null) return;

            for (int i = 0; i < safeZoneInfoViews.Count; i++)
            {
                if (safeZoneInfoViews[i])
                    safeZoneInfoViews[i].Refresh(_zoneController.Index, _spinConfig);
            }
        }

        private void RefreshButtons()
        {
            spinButton.interactable = !spinView.IsChanging && (_spinController.CanSpin || _spinController.IsSpinning);
            skipButton.gameObject.SetActive(_spinController.IsSpinning);
            
            caseView?.SetExitAvailable(CanExit);
            
        }

        private void ApplyBackground(ZoneData zoneData)
        {
            if (!backgroundImage) return;

            foreach (var background in backgroundByZones)
            {
                if(background.ZoneData != zoneData) continue;

                backgroundImage.DOColor(background.BackgroundColor, .1f);
                return;
            }
        }

        
        
        private void ContinueAfterRevive()
        {
            int bombIndex = _spinController.Revive();
            spinView.CloseSlice(bombIndex);
            RefreshButtons();
        }

        public override void Show()
        {
            base.Show();
            SpinSession.OnReviveRequested -= ContinueAfterRevive;
            SpinSession.OnReviveRequested += ContinueAfterRevive;
            ClaimedRewards.OnClaimRequested -= OnClaimRequested;
            ClaimedRewards.OnClaimRequested += OnClaimRequested;
            StartRun();
        }


        public override void Hide()
        {
            SpinSession.OnReviveRequested -= ContinueAfterRevive;
            ClaimedRewards.OnClaimRequested -= OnClaimRequested;
            spinView.Stop();
            zoneView.Release();
            caseView?.Clear();
            _runState?.Restart();
            base.Hide();
        }
        
        public override void Dispose()
        {
            base.Dispose();
            SpinSession.OnReviveRequested -= ContinueAfterRevive;
            ClaimedRewards.OnClaimRequested -= OnClaimRequested;
            spinButton?.onClick.RemoveListener(_Spin);
            skipButton?.onClick.RemoveListener(_Skip);
            caseView?.Dispose();
        }

        private void OnValidate()
        {
            if (!spinButton)
                spinButton = this.FindChild<Button>("ui_spin_button");
            if (!skipButton)
                skipButton = this.FindChild<Button>("ui_animation_skip_panel");
        }
    }
}