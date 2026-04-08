using PrimeTween;
using TMPro;
using UnityEngine;
using _Project.Scripts.Setups.Tooltip;

namespace _Project.Scripts.UI.Tooltip
{
    [RequireComponent(typeof(CanvasGroup))]
    public class TooltipShower : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _tooltipText;
        
        [Space]
        [SerializeField] private float _fadeInDuration = 0.2f;
        [SerializeField] private float _fadeOutDuration = 0.2f;
        [SerializeField] private float _hideDelay = 0.3f;

        private Tween _fadeTween;

        private void Awake()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        public void Show(TooltipData data)
        {
            _fadeTween.Stop();
            
            _tooltipText.text = data.Text;
            
            _fadeTween = Tween.Alpha(_canvasGroup, 1f, _fadeInDuration)
                .OnComplete(target: _canvasGroup, target =>
                {
                    target.interactable = true;
                    target.blocksRaycasts = true;
                });
        }

        public void Hide()
        {
            _fadeTween.Stop();
            
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            _fadeTween = Tween.Alpha(_canvasGroup, 0f, _fadeOutDuration, startDelay: _hideDelay);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }
        }
#endif
    }
}