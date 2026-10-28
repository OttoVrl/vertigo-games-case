using System;
using System.Collections;
using DG.Tweening;
using Runtime.Data;
using Runtime.Enums;
using Runtime.Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class SpinRewardCardView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private RectTransform mainFrameRectTransform;
        [SerializeField] private TMP_Text rewardTitleText;
        [SerializeField] private TMP_Text rewardAmountText;
        [SerializeField] private Image rewardImage;

        [Header("Settings")] 
        [SerializeField] private float showDuration = .2f;
        [SerializeField] private float hideDuration = .2f;
        [SerializeField] private float holdDuration = 2f;
        
        public bool TryShow(BaseCollectibleData rewardData,int amount,Action onShowed,Action onComplete)
        {
            if (!Ensure())
                return false;
            
            mainFrameRectTransform.gameObject.SetActive(true);
            
            rewardTitleText.text = rewardData.DisplayName;
            rewardAmountText.text = "x"+amount.ToString("N0");
            rewardImage.sprite = rewardData.Icon;

            StartCoroutine(ShowAnimation(onShowed, onComplete));
            
            return true;
        }

        public void SetReward(BaseCollectibleData rewardData, int amount)
        {
            if (!Ensure() || !rewardData) return;

            mainFrameRectTransform.gameObject.SetActive(true);
            mainFrameRectTransform.localScale = Vector3.one;
            rewardTitleText.text = rewardData.DisplayName;
            rewardAmountText.text = "x" + amount.ToString("N0");
            rewardImage.sprite = rewardData.Icon;
        }

        private IEnumerator ShowAnimation(Action onShowed,Action onComplete)
        {
            yield return mainFrameRectTransform.DOScale(Vector3.one, showDuration)
                    .SetEase(Ease.OutBack);

            
            onShowed?.Invoke();
            SfxManager.Instance.PlaySFX(SFXType.RewardCardReveal);
            
            yield return new WaitForSeconds(holdDuration);

            yield return Hide().OnComplete(() =>
            {
                mainFrameRectTransform.gameObject.SetActive(false);
                onComplete?.Invoke();
            });
        }

        private bool Ensure()
        {
            if (!mainFrameRectTransform || !rewardAmountText || !rewardImage || !rewardTitleText)
                return false;

            return true;
        }
        
        private Tween Hide()
        {
            return mainFrameRectTransform.transform.DOScale(Vector3.one * .7f,hideDuration)
                .SetEase(Ease.InBack);
        }
    }
}
