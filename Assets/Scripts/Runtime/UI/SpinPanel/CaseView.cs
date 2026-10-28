using System;
using System.Collections.Generic;
using Runtime.Data;
using Runtime.Extensions;
using Runtime.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class CaseView : MonoBehaviour
    {
        [SerializeField] private CaseRewardElement caseRewardElementPrefab;
        [SerializeField] private RectTransform rewardsContainer;
        [SerializeField] private Button exitButton;

        private Action _onExit;

        private readonly Dictionary<BaseCollectibleData, CaseRewardElement> _elements = new();

        public void Initialize(Action onExit)
        {
            _onExit = onExit;
            exitButton.onClick.AddListener(_ExitClicked);
        }

        public CaseRewardElement ShowReward(BaseCollectibleData data)
        {
            if (_elements.TryGetValue(data, out CaseRewardElement element))
                return element;
            
            element = Factory.Create(caseRewardElementPrefab, rewardsContainer);
            element.Initialize(data,0);
            _elements.Add(data, element);
            SortRewards();
            return element;
        }

        private void SortRewards()
        {
            var sorted = new List<BaseCollectibleData>(_elements.Keys);
            sorted.Sort((a, b) =>
            {
                int order = a.CaseOrderIndex.CompareTo(b.CaseOrderIndex);
                if (order != 0)
                    return order;

                return _elements[a].transform.GetSiblingIndex().CompareTo(_elements[b].transform.GetSiblingIndex());
            });

            for (int i = 0; i < sorted.Count; i++)
                _elements[sorted[i]].transform.SetSiblingIndex(i);
        }

        public void SetRewardAmount(BaseCollectibleData data, int amount)
        {
            if (_elements.TryGetValue(data, out CaseRewardElement element))
                element.SetAmount(amount);
        }
        
        
        public void SetExitAvailable(bool available)
        {
            exitButton.interactable = available;
        }

        public void Show()
        {
            
        }

        public void Clear()
        {
            foreach (CaseRewardElement element in _elements.Values)
            {
                if (element)
                    Factory.Destroy(element.gameObject);
            }

            _elements.Clear();
        }

        public void Hide()
        {
            
        }
        
        private void _ExitClicked()
        {
            _onExit?.Invoke();
        }
        
        public void Dispose()
        {
            exitButton.onClick.RemoveListener(_ExitClicked);
            _onExit = null;
        }

        private void OnValidate()
        {
            if (!exitButton)
                exitButton = this.FindChild<Button>("ui_exit_button");
        }
    }
}
