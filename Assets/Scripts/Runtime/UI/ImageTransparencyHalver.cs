using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    [DisallowMultipleComponent]
    public class ImageTransparencyHalver : MonoBehaviour
    {
        //Demoda boş placeholderlı menü sahnesi ekledim orda işe yaramayan placeholderlar için yazıldı.
        [SerializeField] private bool includeInactive = true;

        private readonly List<Image> _images = new List<Image>();
        private readonly List<float> _originalAlphas = new List<float>();
        private bool _isApplied;

        private void OnEnable()
        {
            Apply();
        }

        private void OnDisable()
        {
            Restore();
        }

        private void Apply()
        {
            if (_isApplied)
                return;

            _images.Clear();
            _originalAlphas.Clear();
            GetComponentsInChildren(includeInactive, _images);

            for (int i = 0; i < _images.Count; i++)
            {
                Image image = _images[i];
                Color color = image.color;
                _originalAlphas.Add(color.a);
                color.a *= 0.5f;
                image.color = color;
            }

            _isApplied = true;
        }

        private void Restore()
        {
            if (!_isApplied)
                return;

            for (int i = 0; i < _images.Count; i++)
            {
                Image image = _images[i];
                if (image == null)
                    continue;

                Color color = image.color;
                color.a = _originalAlphas[i];
                image.color = color;
            }

            _images.Clear();
            _originalAlphas.Clear();
            _isApplied = false;
        }
    }
}
