using System;
using DG.Tweening;
using Runtime.Enums;
using Runtime.Managers;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Runtime.UI
{
    public class FlyingReward : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private RectTransform rectTransform;

        private Sequence _sequence;

        public void Play(Sprite sprite, Vector2 startPosition, Vector2 targetPosition, float delay, Action onComplete)
        {
            iconImage.sprite = sprite;
            rectTransform.anchoredPosition = startPosition;
            rectTransform.localScale = Vector3.zero;
            rectTransform.localEulerAngles = Vector3.zero;

            Vector2 scatter = UnityEngine.Random.insideUnitCircle * 70f;
            float spin = UnityEngine.Random.Range(-25f, 25f);

            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.AppendInterval(delay);
            _sequence.Append(rectTransform.DOAnchorPos(startPosition + scatter, 0.28f).SetEase(Ease.OutCubic));
            _sequence.Join(rectTransform.DOScale(1f, 0.28f).SetEase(Ease.OutBack));
            _sequence.Join(rectTransform.DOLocalRotate(new Vector3(0f, 0f, spin), 0.28f));
            _sequence.Append(rectTransform.DOAnchorPos(targetPosition, 0.45f).SetEase(Ease.InQuad));
            _sequence.Join(rectTransform.DOScale(0.8f, 0.45f));
            _sequence.Join(rectTransform.DOLocalRotate(Vector3.zero, 0.45f));
            _sequence.OnComplete(() =>
            {
                SfxManager.Instance.PlaySFX(SFXType.RewardArrive,.5f,Random.Range(.8f,1.2f));
                onComplete?.Invoke();
            });
        }

        private void OnDisable()
        {
            _sequence?.Kill();
            _sequence = null;
        }
    }
}
