using DG.Tweening;
using Runtime.Data;
using Runtime.Enums;
using Runtime.Interface;
using Runtime.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public abstract class BasePopUp : MonoBehaviour,IUIWindow
    {
        [SerializeField] protected RectTransform mainFrameRectTransform;
        [SerializeField] protected Button closeButton;

        public bool IsActivated { get; private set; }
        protected GlobalConfig _globalConfig;

        public virtual void Initialize(GlobalConfig globalConfig)
        {
            this._globalConfig = globalConfig;
            if(closeButton) closeButton.onClick.AddListener(Hide);
            IsActivated = false;
        }

        public virtual void Show()
        {
            mainFrameRectTransform.localScale = Vector3.one * .7f;
            IsActivated = true;
            SetActive(true);
            ShowAnimation();
        }

        public virtual void Hide()
        {
            if (!IsActivated) return;
            
            mainFrameRectTransform.transform.DOScale(Vector3.one * .7f,.25f)
                .OnComplete(CloseComplete)
                .SetEase(Ease.InBack);
            
            StateManager.SetPopUpState(PopUpState.None);
        }

        private void ShowAnimation()
        {
            mainFrameRectTransform.DOScale(Vector3.one, .25f)
                .SetEase(Ease.OutBack);
        }

        private void CloseComplete()
        {
            SetActive(false);
        }

        public virtual void CloseWithoutAnim()
        {
            SetActive(false);
        }
        private void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }

        private void OnDestroy()
        {
            if(closeButton) closeButton.onClick.RemoveAllListeners();
        }
    }
}
