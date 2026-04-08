using _Project.Scripts.Player.Interaction;
using _Project.Scripts.UI.Tasks;
using _Project.Scripts.Utilities.Events;
using Game.Scripts.Utilities.Events;
using PrimeTween;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Printer.Filament
{
    public class FilamentPlacer : MonoBehaviour, IClickable
    {
        [Header("Animation Settings")]
        [SerializeField] private Transform _placerTransform;
        [SerializeField] private Transform _filamentTransform;

        [Header("Line Renderer")]
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private Transform _startPoint;
        [SerializeField] private Transform _targetPoint;

        [Header("Tween Settings")]
        [SerializeField] private Ease _ease = Ease.InOutSine;
        [SerializeField] private Vector3 _rotationAxis = Vector3.up;
        [SerializeField] private float _rotationSpeedDegreesPerSecond = 360f;

        private EventBus _eventBus;
        private TaskManager _taskManager;

        private Sequence _placementSequence;
        private Tween _rotationTween;
        private bool _isPlaced;
        private bool _isFilamentRemovalBlocked;
        private Color _currentFilamentColor = Color.white;

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
        public void Construct(EventBus eventBus, [InjectOptional] TaskManager taskManager = null)
        {
            _eventBus = eventBus;
            _taskManager = taskManager;
        }

        private void OnEnable()
        {
            _eventBus?.Subscribe<OnPrintSafetyLockChanged>(OnPrintSafetyLockChanged);
            _eventBus?.Subscribe<OnFilamentColorChanged>(OnFilamentColorChanged);
        }

        private void OnDisable()
        {
            _eventBus?.Unsubscribe<OnPrintSafetyLockChanged>(OnPrintSafetyLockChanged);
            _eventBus?.Unsubscribe<OnFilamentColorChanged>(OnFilamentColorChanged);
            StopRotation();
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
            if (_isFilamentRemovalBlocked)
            {
                Debug.LogWarning("[FilamentPlacer.OnClick] Нельзя снимать филамент во время печати.");
                _taskManager?.ShowMessage("Нельзя снимать филамент во время печати.");
                return;
            }

            TogglePlacement();
        }

        private void TogglePlacement()
        {
            StopCurrentTween();

            if (_isPlaced)
            {
                SetLineVisible(false);
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
                .Group(Tween.LocalPosition(_placerTransform, IntermediatePosition1, Duration1, _ease))
                .Group(Tween.LocalRotation(_placerTransform, Quaternion.Euler(IntermediateRotation1), Duration1, _ease))
                .Chain(Tween.LocalPosition(_placerTransform, IntermediatePosition2, Duration2, _ease))
                .Chain(Tween.LocalPosition(_placerTransform, IntermediatePosition3, Duration3, _ease))
                .Chain(Tween.LocalPosition(_placerTransform, FinalPosition, Duration4, _ease))
                .OnComplete(() => OnPlacementComplete(true));
        }

        private void PlayRemovalSequence()
        {
            _placementSequence = Sequence.Create()
                .Group(Tween.LocalPosition(_placerTransform, IntermediatePosition3, Duration4, _ease))
                .Chain(Tween.LocalPosition(_placerTransform, IntermediatePosition2, Duration3, _ease))
                .Chain(Tween.LocalPosition(_placerTransform, IntermediatePosition1, Duration2, _ease))
                .Group(Tween.LocalPosition(_placerTransform, StartPosition, Duration1, _ease))
                .Group(Tween.LocalRotation(_placerTransform, Quaternion.Euler(StartRotation), Duration1, _ease))
                .OnComplete(() => OnPlacementComplete(false));
        }

        private void OnPlacementComplete(bool placed)
        {
            SetLineVisible(placed);

            if (placed)
            {
                _taskManager?.CompleteStep(TaskStepType.InsertFilament);
            }
            else
            {
                _taskManager?.UncompleteStep(TaskStepType.InsertFilament);
            }

            _eventBus.Publish(new OnInteractableStateChangedEvent<PrinterElement>(PrinterElement.Filament, placed));
            _eventBus.Publish(new OnFilamentStateChanged { IsPlaced = placed });
        }

        private void StopCurrentTween()
        {
            if (_placementSequence.isAlive)
            {
                _placementSequence.Stop();
            }
        }

        private void OnPrintSafetyLockChanged(OnPrintSafetyLockChanged evt)
        {
            _isFilamentRemovalBlocked = evt.IsFilamentRemovalBlocked;
        }

        public void BeginRotation()
        {
            if (_placerTransform == null)
            {
                return;
            }

            if (_rotationTween.isAlive)
            {
                return;
            }

            StartRotationLoop();
        }

        public void StopRotation()
        {
            if (_rotationTween.isAlive)
            {
                _rotationTween.Stop();
            }
        }

        private void OnDestroy()
        {
            StopCurrentTween();
            StopRotation();
        }

        private void Start()
        {
            InitializeLineRenderer();
            SetLineVisible(_isPlaced);
            _eventBus.Publish(new OnFilamentStateChanged { IsPlaced = _isPlaced });
        }

        private void StartRotationLoop()
        {
            if (_filamentTransform == null)
            {
                return;
            }

            var axis = _rotationAxis.sqrMagnitude > 0f ? _rotationAxis.normalized : Vector3.up;
            var duration = 360f / Mathf.Max(1f, _rotationSpeedDegreesPerSecond);
            var targetEuler = _filamentTransform.localEulerAngles + (axis * 360f);

            _rotationTween = Tween.LocalEulerAngles(_filamentTransform, 
                _filamentTransform.localEulerAngles,
                targetEuler,
                duration,
                cycles: -1,
                cycleMode: CycleMode.Incremental,
                ease: _ease
            );
        }

        private void OnFilamentColorChanged(OnFilamentColorChanged evt)
        {
            _currentFilamentColor = evt.Color;
        }

        private void InitializeLineRenderer()
        {
            if (_lineRenderer == null)
            {
                return;
            }

            _lineRenderer.positionCount = 2;
            _lineRenderer.enabled = false;
            UpdateLinePositions();
        }

        private void UpdateLinePositions()
        {
            if (_lineRenderer == null || _startPoint == null || _targetPoint == null)
            {
                return;
            }

            _lineRenderer.SetPosition(0, _startPoint.position);
            _lineRenderer.SetPosition(1, _targetPoint.position);
        }

        private void SetLineVisible(bool isVisible)
        {
            if (_lineRenderer == null)
            {
                return;
            }

            var canRender = isVisible && _startPoint != null && _targetPoint != null;
            _lineRenderer.enabled = canRender;

            if (canRender)
            {
                UpdateLinePositions();
            }
        }
    }
}
