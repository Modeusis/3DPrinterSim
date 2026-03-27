using _Project.Scripts.Setups.Animation;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Project.Scripts.UI
{
    [RequireComponent(typeof(Image))]
    public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField, Space] private Image _buttonImage;
        [SerializeField] private TMP_Text _buttonText;
        
        [SerializeField, Space] private Vector3AnimationProperty _hoverProperty;
        [SerializeField] private Vector3AnimationProperty _clickProperty;
        [SerializeField] private Vector3AnimationProperty _idleProperty;
        [SerializeField] private Vector3AnimationProperty _disableProperty;
        
        private Tween _hoverTween;
        private Tween _clickTween;
        private Tween _idleTween;
        
        private Sequence _idleSequence;
        private Sequence _disableSequence;
        
        private bool _isActive = true;
        
        [field: SerializeField] public UnityEvent OnClick { get; private set; }
        [field: SerializeField] public UnityEvent OnHover { get; private set; }
        [field: SerializeField] public UnityEvent OnUnhover { get; private set; } 

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_isActive)
                return;
            
            StopTweens();
            
            _hoverTween = Tween.Scale(
                transform, 
                _hoverProperty.Value, 
                _hoverProperty.Duration, 
                _hoverProperty.Ease
            );
            
            OnHover?.Invoke();
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isActive)
                return;
            
            StopTweens();
            
            _clickTween = Tween.Scale(
                transform, 
                _clickProperty.Value, 
                _clickProperty.Duration, 
                _clickProperty.Ease
            );
            
            OnClick?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isActive)
                return;
            
            StopTweens();
            
            _hoverTween = Tween.Scale(
                transform, 
                _hoverProperty.Value, 
                _hoverProperty.Duration, 
                _hoverProperty.Ease
            );
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_isActive)
                return;
            
            StopTweens();

            _idleTween = Tween.Scale(
                transform, 
                _idleProperty.Value, 
                _idleProperty.Duration, 
                _idleProperty.Ease
            );
            
            OnUnhover?.Invoke();
        }
        
        public void SetActiveButton(bool isActive)
        {
            if (_isActive == isActive)
                return;
            
            StopTweens();
            
            _isActive = isActive;

            if (_isActive)
            {
                _idleSequence = Sequence.Create();
            
                _idleSequence.Group(Tween.Scale(transform, _idleProperty.Value, _idleProperty.Duration, _idleProperty.Ease));
                _idleSequence.Group(Tween.Alpha(_buttonImage, 1f, _idleProperty.Duration, _idleProperty.Ease));
                if (_buttonText != null)
                {
                    _idleSequence.Group(Tween.Alpha(_buttonText, 1f, _idleProperty.Duration, _idleProperty.Ease));
                }
                
                _idleSequence.OnComplete(() =>
                {
                    _buttonImage.color = Color.white;
                    if (_buttonText != null)
                    {
                        _buttonText.color = Color.white;
                    }
                });
                
                return;
            }
            
            _disableSequence = Sequence.Create();
            
            _disableSequence.Group(Tween.Scale(transform, _disableProperty.Value, _disableProperty.Duration, _disableProperty.Ease));
            _disableSequence.Group(Tween.Alpha(_buttonImage, 0.5f, _disableProperty.Duration, _disableProperty.Ease));
            if (_buttonText != null)
            {
                _disableSequence.Group(Tween.Alpha(_buttonText, 0.5f, _disableProperty.Duration, _disableProperty.Ease));
            }
            
            _disableSequence.OnComplete(() =>
            {
                _buttonImage.color = new Color(1f, 1f, 1f, 0.5f);
                if (_buttonText != null)
                {
                    _buttonText.color = new Color(1f, 1f, 1f, 0.5f);
                }
            });
        }

        private void StopTweens()
        {
            _hoverTween.Stop();
            _clickTween.Stop();
            _idleTween.Stop();
            _idleSequence.Stop();
            _disableSequence.Stop();
        }

        private void OnDestroy()
        {
            StopTweens();
            
            OnClick?.RemoveAllListeners();
            OnHover?.RemoveAllListeners();
            OnUnhover?.RemoveAllListeners();
        }
    }
}