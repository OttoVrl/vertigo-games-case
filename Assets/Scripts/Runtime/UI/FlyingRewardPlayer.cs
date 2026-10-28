using System;
using Runtime.Managers;
using UnityEngine;

namespace Runtime.UI
{
    public class FlyingRewardPlayer : MonoBehaviour
    {
        public static FlyingRewardPlayer Instance { get; private set; }

        [SerializeField] private FlyingReward rewardPrefab;
        [SerializeField] private RectTransform layer;
        [SerializeField] private Canvas canvas;
        [SerializeField, Min(1)] private int minCount = 1;
        [SerializeField, Min(1)] private int maxCount = 8;
        [SerializeField, Min(1)] private int amountPerIcon = 40;

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void Play(Sprite icon, int amount, RectTransform from, RectTransform to, Action onArrived = null)
        {
            if (!icon || amount <= 0 || !from || !to || !rewardPrefab || !layer)
            {
                onArrived?.Invoke();
                return;
            }

            int count = Mathf.Clamp(Mathf.CeilToInt(amount / (float)amountPerIcon), minCount, maxCount);
            int arrivedCount = 0;
            Vector2 startPosition = ToLayerPosition(from.position);
            Vector2 targetPosition = ToLayerPosition(to.position);

            for (int i = 0; i < count; i++)
            {
                FlyingReward reward = Factory.Create(rewardPrefab, layer);
                float delay = i * 0.05f;
                reward.Play(icon, startPosition, targetPosition, delay, () =>
                {
                    Factory.Destroy(reward.gameObject);
                    arrivedCount++;
                    if (arrivedCount >= count)
                        onArrived?.Invoke();
                });
            }
        }

        private Vector2 ToLayerPosition(Vector3 worldPosition)
        {
            Camera camera = null;
            if (canvas && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                camera = canvas.worldCamera;

            Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(camera, worldPosition);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(layer, screenPosition, camera, out Vector2 localPosition);
            return localPosition;
        }
    }
}
