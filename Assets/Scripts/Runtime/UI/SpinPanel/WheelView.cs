using System;
using System.Collections.Generic;
using DG.Tweening;
using Runtime.Enums;
using Runtime.Interface;
using Runtime.Managers;
using Runtime.Spin;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class WheelView : MonoBehaviour,ISkippableTween
    {
        [SerializeField] private WheelSliceView wheelSliceViewPrefab;
        [SerializeField] private Image wheelImage;
        [SerializeField] private Image wheelIndicatorImage;
        [SerializeField] private RectTransform wheelRectTransform;
        [SerializeField] private List<WheelSliceView> sliceViews = new  List<WheelSliceView>();
        
        [SerializeField] private float sliceRadius = 180f;
        [SerializeField] private float startAngle;
        [SerializeField, Min(1)] private int gizmoSliceCount = 8;
        
        [Header("Spin Animation Settings")]
        [SerializeField,Min(1)] private int fullSpinCount = 4;
        [SerializeField, Min(.1f)] private float spinDuration = 3f;

        [Header("Zone Change")]
        [SerializeField, Min(0.05f)] private float zoneSlideDuration = 0.3f;
        
        private readonly TweenSkipper _skipper = new  TweenSkipper();
        private Tween _spinTween;
        private Tween _zoneTween;
        private CanvasGroup _canvasGroup;
        private bool _finished;
        private bool _hasWheel;
        private bool _restCaptured;
        private float _restX;
        private float _spinTravel;
        private float _sliceAngle;
        private int _spinTick;
        private Ease _spinEase;
        
        public bool CanSkip => _skipper.CanSkip;
        public bool IsChanging { get; private set; }

        public void Initialize(Sprite wheelSprite, Sprite indicatorSprite, IReadOnlyList<RolledSlice> slices, Action onReady = null)
        {
            EnsureCanvasGroup();
            CaptureRest();

            if (!_hasWheel)
            {
                KillSpin();
                ApplyContent(wheelSprite, indicatorSprite, slices);
                ResetPresentation();
                _hasWheel = true;
                IsChanging = false;
                onReady?.Invoke();
                return;
            }

            PlayZoneChange(wheelSprite, indicatorSprite, slices, onReady);
        }

        public void SpinTo(int sliceIndex, int sliceCount,Action onComplete)
        {
            _spinTween?.Kill();
            _finished = false;
            
            float sliceAngle = 360f/sliceCount;
            float startZ = wheelRectTransform.localEulerAngles.z%360f;
            wheelRectTransform.localEulerAngles = new Vector3(0f, 0f, startZ);

            float targetZ = sliceIndex * sliceAngle - fullSpinCount * 360f;
            _sliceAngle = sliceAngle;
            _spinTravel = Mathf.Abs(targetZ - startZ);
            _spinTick = 0;
            _spinEase = Ease.OutCirc;

            _spinTween = wheelRectTransform
                .DORotate(new Vector3(0f, 0f, targetZ), spinDuration, RotateMode.FastBeyond360)
                .SetEase(_spinEase)
                .OnUpdate(TickSpin)
                .OnComplete(() => Finish(onComplete));
            
            _skipper.Play(_spinTween);
        }
        
        public void Skip()
        {
            _skipper.TrySkip();
        }

        public void Stop()
        {
            KillSpin();
            _zoneTween?.Kill();
            _zoneTween = null;
            IsChanging = false;
            _hasWheel = false;
            ResetPresentation();
        }

        public void CloseSlice(int index)
        {
            if (index < 0 || index >= sliceViews.Count) return;
            sliceViews[index].gameObject.SetActive(false);
        }

        private void TickSpin()
        {
            if (_spinTween == null || !_spinTween.IsActive() || _sliceAngle <= 0f) return;

            float eased = DOVirtual.EasedValue(0f, 1f, _spinTween.ElapsedPercentage(), _spinEase);
            int step = Mathf.FloorToInt(eased * _spinTravel / _sliceAngle);
            if (step <= _spinTick) return;

            _spinTick = step;
            float pitch = Mathf.Lerp(0.78f, 1.34f, 1f - eased) + UnityEngine.Random.Range(-0.07f, 0.07f);
            SfxManager.Instance.PlaySFX(SFXType.SpinWheel, pitch: Mathf.Clamp(pitch, 0.7f, 1.45f));
        }

        private void PlayZoneChange(Sprite wheelSprite, Sprite indicatorSprite, IReadOnlyList<RolledSlice> slices, Action onReady)
        {
            _zoneTween?.Kill();
            KillSpin();
            ResetPresentation();

            IsChanging = true;
            float distance = SlideDistance();

            Sequence sequence = DOTween.Sequence();
            sequence.Join(wheelRectTransform.DOAnchorPosX(_restX - distance, zoneSlideDuration).SetEase(Ease.InCubic));
            sequence.Join(_canvasGroup.DOFade(0f, zoneSlideDuration));
            sequence.AppendCallback(() =>
            {
                ApplyContent(wheelSprite, indicatorSprite, slices);
                SetAnchoredX(_restX + distance);
                _canvasGroup.alpha = 0f;
            });
            sequence.Append(wheelRectTransform.DOAnchorPosX(_restX, zoneSlideDuration).SetEase(Ease.OutCubic));
            sequence.Join(_canvasGroup.DOFade(1f, zoneSlideDuration));
            sequence.OnComplete(() =>
            {
                IsChanging = false;
                _zoneTween = null;
                onReady?.Invoke();
            });

            _zoneTween = sequence;
        }

        private void ApplyContent(Sprite wheelSprite, Sprite indicatorSprite, IReadOnlyList<RolledSlice> slices)
        {
            wheelRectTransform.localRotation = Quaternion.identity;
            if (wheelSprite) wheelImage.sprite = wheelSprite;
            if (indicatorSprite && wheelIndicatorImage) wheelIndicatorImage.sprite = indicatorSprite;

            while (sliceViews.Count < slices.Count)
            {
                WheelSliceView element = Factory.Create(wheelSliceViewPrefab, wheelRectTransform);
                sliceViews.Add(element);
            }

            for (int i = 0; i < sliceViews.Count; i++)
                sliceViews[i].gameObject.SetActive(false);

            if (slices.Count != sliceViews.Count)
                Debug.LogError("slice count mismatch");

            for (int i = 0; i < slices.Count; i++)
            {
                RectTransform rect = sliceViews[i].transform as RectTransform;
                WheelSliceLAyout.GetPose(i, slices.Count, sliceRadius, startAngle, out Vector2 position, out float rotationZ);

                rect.anchoredPosition = position;
                rect.localEulerAngles = new Vector3(0f, 0f, rotationZ);
                sliceViews[i].gameObject.SetActive(true);
                sliceViews[i].Initialize(slices[i]);
            }
        }

        private void EnsureCanvasGroup()
        {
            if (_canvasGroup) return;

            _canvasGroup = wheelRectTransform.GetComponent<CanvasGroup>();
            if (!_canvasGroup)
                _canvasGroup = wheelRectTransform.gameObject.AddComponent<CanvasGroup>();

            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        private void CaptureRest()
        {
            if (_restCaptured) return;
            _restX = wheelRectTransform.anchoredPosition.x;
            _restCaptured = true;
        }

        private float SlideDistance()
        {
            float width = wheelRectTransform.rect.width;
            if (width < 1f && wheelRectTransform.parent is RectTransform parent)
                width = parent.rect.width;
            if (width < 1f)
                width = 1000f;
            return width;
        }

        private void SetAnchoredX(float x)
        {
            Vector2 position = wheelRectTransform.anchoredPosition;
            position.x = x;
            wheelRectTransform.anchoredPosition = position;
        }

        private void ResetPresentation()
        {
            if (!wheelRectTransform) return;
            if (_restCaptured)
                SetAnchoredX(_restX);
            if (_canvasGroup)
                _canvasGroup.alpha = 1f;
        }

        private void KillSpin()
        {
            _spinTween?.Kill();
            _skipper.Clear();
            _finished = true;
        }

        private void Finish(Action onComplete)
        {
            if(_finished) return;
            
            _finished = true;
            _skipper.Clear();
            onComplete?.Invoke();
        }

        private void OnDisable()
        {
            _zoneTween?.Kill();
            _zoneTween = null;
            if(_spinTween != null && _spinTween.IsActive() && !_spinTween.IsComplete())
                _spinTween.Complete(true);
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!wheelRectTransform) return;
            int count = Application.isPlaying && sliceViews.Count > 0 ? sliceViews.Count : gizmoSliceCount;
            if (count <= 0) return;
            for (int i = 0; i < count; i++)
            {
                WheelSliceLAyout.GetPose(i, count, sliceRadius, startAngle, out Vector2 position, out float rotationZ);
                Vector3 center = wheelRectTransform.TransformPoint(Vector3.zero);
                Vector3 point = wheelRectTransform.TransformPoint(position);
                Vector3 direction = Quaternion.Euler(0f, 0f, rotationZ) * Vector3.up;
                Vector3 arrow = wheelRectTransform.TransformVector(direction) * 30f;
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(center, point);
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(point, point + arrow);
                Gizmos.DrawSphere(point, 4f);
            }
        }
#endif
    }
}
