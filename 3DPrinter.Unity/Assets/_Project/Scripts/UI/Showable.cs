using _Project.Scripts.Setups.Animation;
using PrimeTween;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.UI
{
    public class Showable : MonoBehaviour
    {
        [SerializeField, Space] private RectTransform _showRect;
        [SerializeField] private Vector2AnimationProperty _openAnimation;
        [SerializeField] private Vector2AnimationProperty _closeAnimation;
        
        private Tween _showTween;
        private bool _isOpen;

        private void Awake()
        {
            HideForced();
        }

        public void Toggle()
        {
            if (!_isOpen)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
        
        public void Show()
        {
            if (_isOpen)
                return;
            
            ClearTween();
            _showTween = Tween.UIAnchoredPosition(_showRect, _openAnimation.Value, _openAnimation.Duration, _openAnimation.Ease);
            
            _isOpen = true;
        }
        
        public void Hide()
        {
            if (!_isOpen)
                return;
            
            ClearTween();
            _showTween = Tween.UIAnchoredPosition(_showRect, _closeAnimation.Value, _closeAnimation.Duration, _closeAnimation.Ease);
            
            _isOpen = false;
        }
        
        public void HideForced()
        {
            ClearTween();
            
            if (_showRect != null)
            {
                _showRect.anchoredPosition = _closeAnimation.Value;
            }
            
            _isOpen = false;
        }

        private void ClearTween()
        {
            _showTween.Stop();
        }
    }
}