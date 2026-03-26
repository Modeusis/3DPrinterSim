using _Project.Scripts.Player.Interaction;
using Game.Scripts.Utilities.Events;
using PrimeTween;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Printer.Filament
{
    public class FilamentPlacer : MonoBehaviour, IClickable
    {
        [Header("Animation Settings")]
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _filamentTransform;
        
        [Header("Tween Settings")]
        [SerializeField] private Ease _ease = Ease.InOutSine;
        
        private EventBus _eventBus;
        
        private Sequence _placementSequence;
        private bool _isPlaced;
        private int _isRotatingHash;
        
        private static readonly Vector3 StartPosition = new Vector3(-0.6988287f, 1.5209386f, 1.2248555f);
        private static readonly Vector3 StartRotation = new Vector3(-90f, 0f, 0f);
        
        private static readonly Vector3 IntermediatePosition1 = new Vector3(-0.894f, 1.957f, 1.225f);
        private static readonly Vector3 IntermediateRotation1 = new Vector3(0f, -90f, 0f);
        
        private static readonly Vector3 IntermediatePosition2 = new Vector3(-0.894f, 1.957f, -0.015f);
        
        private static readonly Vector3 IntermediatePosition3 = new Vector3(-0.633f, 1.957f, -0.015f);
        
        private static readonly Vector3 FinalPosition = new Vector3(-0.633f, 1.931f, -0.015f);
        
        private const float Duration1 = 1.5f;
        private const float Duration2 = 1.4833333f;
        private const float Duration3 = 0.4833334f;
        private const float Duration4 = 0.1666667f;

        [Inject]
        public void Construct(EventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void Awake()
        {
            _isRotatingHash = Animator.StringToHash("IsRotating");
        }

        public void OnBeginHover()
        {
            _eventBus.Publish(new OnInteractableHoverEvent("Filament", true));
        }

        public void OnEndHover()
        {
            _eventBus.Publish(new OnInteractableHoverEvent("Filament", false));
        }

        public void OnClick()
        {
            TogglePlacement();
        }

        private void TogglePlacement()
        {
            StopCurrentTween();
            
            if (_isPlaced)
            {
                PlayRemovalSequence();
            }
            else
            {
                PlayPlacementSequence();
            }
            
            _isPlaced = !_isPlaced;
        }

        private void PlayPlacementSequence()
        {
            _placementSequence = Sequence.Create()
                .Group(Tween.LocalPosition(_filamentTransform, IntermediatePosition1, Duration1, _ease))
                .Group(Tween.LocalRotation(_filamentTransform, Quaternion.Euler(IntermediateRotation1), Duration1, _ease))
                .Chain(Tween.LocalPosition(_filamentTransform, IntermediatePosition2, Duration2, _ease))
                .Chain(Tween.LocalPosition(_filamentTransform, IntermediatePosition3, Duration3, _ease))
                .Chain(Tween.LocalPosition(_filamentTransform, FinalPosition, Duration4, _ease))
                .OnComplete(() => OnPlacementComplete(true));
        }

        private void PlayRemovalSequence()
        {
            _placementSequence = Sequence.Create()
                .Group(Tween.LocalPosition(_filamentTransform, IntermediatePosition3, Duration4, _ease))
                .Chain(Tween.LocalPosition(_filamentTransform, IntermediatePosition2, Duration3, _ease))
                .Chain(Tween.LocalPosition(_filamentTransform, IntermediatePosition1, Duration2, _ease))
                .Group(Tween.LocalPosition(_filamentTransform, StartPosition, Duration1, _ease))
                .Group(Tween.LocalRotation(_filamentTransform, Quaternion.Euler(StartRotation), Duration1, _ease))
                .OnComplete(() => OnPlacementComplete(false));
        }

        private void OnPlacementComplete(bool placed)
        {
            _eventBus.Publish(new OnInteractableStateChangedEvent<PrinterElement>(PrinterElement.Filament, placed));
        }

        private void StopCurrentTween()
        {
            if (_placementSequence.isAlive)
            {
                _placementSequence.Stop();
            }
        }

        public void BeginRotation()
        {
            _animator.SetBool(_isRotatingHash, true);
        }

        public void StopRotation()
        {
            _animator.SetBool(_isRotatingHash, false);
        }

        private void OnDestroy()
        {
            StopCurrentTween();
        }
    }
}
