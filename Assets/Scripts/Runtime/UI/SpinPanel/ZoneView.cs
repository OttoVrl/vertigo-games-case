using System.Collections.Generic;
using DG.Tweening;
using Runtime.Data;
using Runtime.Managers;
using Runtime.Spin;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class ZoneView : MonoBehaviour
    {
        private const int StartStepIndex = 6;
        private const float ElementWidth = 65f;
        private const float BadgeSlideDuration = 0.25f;
        private static readonly Color DefaultZoneColor = Color.white;
        
        [SerializeField] private ZoneElement zoneElementPrefab;
        [SerializeField] private RectTransform elementsContainer;
        [SerializeField] private RectTransform currentFrame;
        [SerializeField] private TMP_Text currentZoneText;
        [SerializeField] private Image currentZoneImage;
        
        [SerializeField] private List<ZoneElement> _elements = new();

        private float _originX;
        private float _stepWidth;
        private int _step;
        private Tween _slideTween;
        private Sequence _slideCurrentFrameTween;
        private IReadOnlyList<ZoneStep> _zones;
        private bool _badgeRestCaptured;
        private bool _textRidesOnImage;
        private float _imageRestX;
        private float _textRestX;

        public void Show(IReadOnlyList<ZoneStep> zones, int startZone)
        {
            _slideCurrentFrameTween?.Kill();
            _slideCurrentFrameTween = null;
            _zones = zones;
            CreateLine(zones);

            _step = Mathf.Max(0, startZone - 1);
            SetX(_originX - _step * _stepWidth);
            CaptureBadgeRest();
            ResetBadge();
            ApplyBadge();
        }

        public void NextStep()
        {
            _step++;
            SlideTo(_originX - _step * _stepWidth);
            PlayBadgeChange();
        }

        private void CreateLine(IReadOnlyList<ZoneStep> zones)
        {
            if (_elements.Count > 0) return;

            ClearContainer();

            for (int i = 0; i < StartStepIndex; i++)
            {
                ZoneElement emptyElement = Factory.Create(zoneElementPrefab, elementsContainer);
                emptyElement.SetEmpty();
                _elements.Add(emptyElement);
            }

            for (int i = 0; i < zones.Count; i++)
            {
                ZoneElement element = Factory.Create(zoneElementPrefab, elementsContainer);
                element.Initialize(zones[i]);
                _elements.Add(element);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(elementsContainer);
            _stepWidth = MeasureStep();
            PlaceSlotOnFrame(StartStepIndex);
            _originX = elementsContainer.anchoredPosition.x;
        }

        private float MeasureStep()
        {
            if (_elements.Count >= 2)
            {
                float distance = ((RectTransform)_elements[1].transform).anchoredPosition.x
                                 - ((RectTransform)_elements[0].transform).anchoredPosition.x;

                if (Mathf.Abs(distance) > 1f)
                    return distance;
            }

            var layout = elementsContainer.GetComponent<HorizontalLayoutGroup>();
            float spacing = layout ? layout.spacing : 0f;
            return ElementWidth + spacing;
        }

        private void PlaceSlotOnFrame(int slotIndex)
        {
            RectTransform slot = _elements[slotIndex].transform as RectTransform;
            Vector3 slotCenter = slot.TransformPoint(slot.rect.center);
            Vector3 frameCenter = currentFrame.TransformPoint(currentFrame.rect.center);

            elementsContainer.position += new Vector3(frameCenter.x - slotCenter.x, 0f, 0f);
        }

        private void ClearContainer()
        {
            for (int i = elementsContainer.childCount - 1; i >= 0; i--)
            {
                Transform child = elementsContainer.GetChild(i);
                child.SetParent(null, false);
                Destroy(child.gameObject);
            }
        }

        private void SlideTo(float targetX)
        {
            _slideTween?.Kill();
            
            _slideTween = elementsContainer
                .DOAnchorPosX(targetX, 0.25f)
                .SetEase(Ease.OutCubic);

            
        }

        private void SetX(float x)
        {
            _slideTween?.Kill();
            
            var position = elementsContainer.anchoredPosition;
            position.x = x;
            elementsContainer.anchoredPosition = position;
        }
        
        public void Release()
        {
            _slideTween?.Kill();
            _slideTween = null;
            _slideCurrentFrameTween?.Kill();
            _slideCurrentFrameTween = null;
            ResetBadge();
            _zones = null;

            for (int i = _elements.Count - 1; i >= 0; i--)
            {
                if (_elements[i])
                    Factory.Destroy(_elements[i].gameObject);
            }

            _elements.Clear();
            _step = 0;
        }

        private void PlayBadgeChange()
        {
            RectTransform imageRect = currentZoneImage ? currentZoneImage.rectTransform : null;
            RectTransform textRect = currentZoneText ? currentZoneText.rectTransform : null;
            bool slideText = textRect && !_textRidesOnImage;
            if (!imageRect && !slideText) return;

            _slideCurrentFrameTween?.Kill();
            ResetBadge();

            float imageDistance = SlideDistance(imageRect);
            float textDistance = SlideDistance(textRect);

            Sequence sequence = DOTween.Sequence();
            bool hasExit = false;
            AddParallel(sequence, imageRect, _imageRestX - imageDistance, ref hasExit);
            AddParallel(sequence, slideText ? textRect : null, _textRestX - textDistance, ref hasExit);
            sequence.AppendCallback(() =>
            {
                ApplyBadge();
                if (imageRect)
                    SetAnchoredX(imageRect, _imageRestX + imageDistance);
                if (slideText)
                    SetAnchoredX(textRect, _textRestX + textDistance);
            });

            Tween imageIn = imageRect
                ? imageRect.DOAnchorPosX(_imageRestX, BadgeSlideDuration).SetEase(Ease.OutCubic)
                : null;
            Tween textIn = slideText
                ? textRect.DOAnchorPosX(_textRestX, BadgeSlideDuration).SetEase(Ease.OutCubic)
                : null;

            if (imageIn != null)
                sequence.Append(imageIn);
            if (textIn != null)
            {
                if (imageIn != null)
                    sequence.Join(textIn);
                else
                    sequence.Append(textIn);
            }

            _slideCurrentFrameTween = sequence;
        }

        private static void AddParallel(Sequence sequence, RectTransform rect, float targetX, ref bool hasTween)
        {
            if (!rect) return;

            Tween tween = rect.DOAnchorPosX(targetX, BadgeSlideDuration).SetEase(Ease.InCubic);
            if (!hasTween)
                sequence.Append(tween);
            else
                sequence.Join(tween);

            hasTween = true;
        }

        private void ApplyBadge()
        {
            if (_zones == null || _step < 0 || _step >= _zones.Count) return;

            ZoneStep step = _zones[_step];
            if (currentZoneText)
                currentZoneText.text = step.Index.ToString();
            if (currentZoneImage)
                currentZoneImage.color = ColorFor(step.Data);
        }

        private static Color ColorFor(ZoneData data)
        {
            if (!data || data.StepInterval <= 1)
                return DefaultZoneColor;
            return data.ZoneColor;
        }

        private void CaptureBadgeRest()
        {
            if (_badgeRestCaptured) return;

            if (currentZoneImage)
                _imageRestX = currentZoneImage.rectTransform.anchoredPosition.x;
            if (currentZoneText)
                _textRestX = currentZoneText.rectTransform.anchoredPosition.x;

            _textRidesOnImage = currentZoneText && currentZoneImage &&
                                currentZoneText.transform.IsChildOf(currentZoneImage.transform);
            _badgeRestCaptured = true;
        }

        private void ResetBadge()
        {
            if (!_badgeRestCaptured) return;

            if (currentZoneImage)
                SetAnchoredX(currentZoneImage.rectTransform, _imageRestX);
            if (currentZoneText && !_textRidesOnImage)
                SetAnchoredX(currentZoneText.rectTransform, _textRestX);
        }

        private static float SlideDistance(RectTransform rect)
        {
            if (!rect) return 0f;

            if (rect.parent is RectTransform parent && parent.rect.width > 1f)
                return parent.rect.width;
            if (rect.rect.width > 1f)
                return rect.rect.width;
            return 400f;
        }

        private static void SetAnchoredX(RectTransform rect, float x)
        {
            Vector2 position = rect.anchoredPosition;
            position.x = x;
            rect.anchoredPosition = position;
        }

        private void Hide()
        {
            
        }
        
        
    }
}
