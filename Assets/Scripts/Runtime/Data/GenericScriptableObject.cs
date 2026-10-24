
using UnityEngine;

namespace Runtime.Data
{
    public abstract class GenericScriptableObject<T> : CustomScriptableObject
    {
        public delegate void EventHandler(T updatedValue);

        public event EventHandler UpdateEvent;
        public event EventHandler ResetEvent;

        [SerializeField] private T defaultValue;
        
        [SerializeField] private T value;

        public T Value
        {
            get => value;
            set
            {
                this.value = value;
                UpdateEvent?.Invoke(value);
            }
        }
        
        public T DefaultValue => defaultValue;

        public override void Reset()
        {
            value = defaultValue;
            ResetEvent?.Invoke(defaultValue);
        }
    }
}
