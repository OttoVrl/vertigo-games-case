using Runtime.Data;
using Runtime.Interface;
using UnityEngine;

namespace Runtime.UI
{
    public class BasePanel : MonoBehaviour,IUIWindow
    {
        protected GlobalConfig _globalConfig;
        
        public bool IsActivated { get; private set;}

        public virtual void Initialize(GlobalConfig globalConfig)
        {
            this._globalConfig = globalConfig;
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }


        public virtual void Dispose(){}
    }
}
