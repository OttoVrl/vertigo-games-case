using DG.Tweening;
using Runtime.Data;
using Runtime.Enums;
using Runtime.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class CaseRewardElement : MonoBehaviour
    {
        [SerializeField] private BaseCollectibleData data;
        
        [Header("UI References")]
        [SerializeField] private Image rewardImage;
        [SerializeField] private TMP_Text rewardAmountText;
        [SerializeField] private CanvasGroup canvasGroup;

        private float startYDeltaSize;
        private RectTransform rectTransform;
        private Tween _amountTween;
        private int _shownAmount;
        
        public void Initialize(BaseCollectibleData data,int amount)
        {
            this.data = data;
            rectTransform = GetComponent<RectTransform>();
            _shownAmount = amount;
            
            canvasGroup.alpha = 0;
            startYDeltaSize = rectTransform.sizeDelta.y;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 0);
            rewardAmountText.text = "x" + amount.ToString("N0");
            rewardImage.sprite = data.Icon;
            
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x,0);

            rectTransform.DOSizeDelta(new Vector2(rectTransform.sizeDelta.x, startYDeltaSize), .2f).OnComplete(() =>
            {
                canvasGroup.DOFade(1, .1f);
            });

        }

        public void SetAmount(int amount)
        {
            _amountTween?.Kill();

            int from = _shownAmount;
            SfxManager.Instance.PlaySFX(SFXType.RewardSetAmount);
            _amountTween = DOTween.To(() => from, value =>
            {
                from = value;
                _shownAmount = value;
                rewardAmountText.text = "x" + value.ToString("N0");
            }, amount, 0.4f).SetEase(Ease.OutCubic);

            rewardAmountText.rectTransform.DOKill(true);
            rewardAmountText.rectTransform.DOPunchScale(Vector3.one * 0.25f, 0.3f, 5, 0.6f);
        }

        public RectTransform GetIconRect()
        {
            return rewardImage.rectTransform;
        }
        
        private void OnDisable()
        {
            _amountTween?.Kill();
        }
    }
}