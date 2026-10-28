using DG.Tweening;
using UnityEngine;

namespace Runtime.UI
{
    public class TweenSkipper
    {
        private Tween _tween;
        private Tween _armTween;
        private bool _armed;

        public bool CanSkip => _armed && _tween != null && _tween.IsActive() && !_tween.IsComplete();

        public void Play(Tween tween)
        {
            if(_tween != null && _tween.IsActive() && _tween.IsComplete())
                _tween.Complete(true);
            
            _armTween?.Kill();
            _tween = tween;
            _armed = false;
            
            //animi başlatan tıklama skip sayılmasın diye küçük bir işlem
            _armTween = DOVirtual.DelayedCall(0f,()=>_armed =true).SetUpdate(true);
        }

        public bool TrySkip()
        {
            if (!CanSkip) return false;
            
            Tween tween = _tween;
            Clear();
            tween.Complete(true);
            return true;
        }

        public void Clear()
        {
            _armTween?.Kill();
            _armTween = null;
            _tween = null;
            _armed = false;
        }
    }
}
