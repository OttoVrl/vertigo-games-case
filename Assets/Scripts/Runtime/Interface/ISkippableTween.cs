using UnityEngine;

namespace Runtime.Interface
{
    public interface ISkippableTween
    {
        bool CanSkip { get; }
        void Skip();
    }
}
