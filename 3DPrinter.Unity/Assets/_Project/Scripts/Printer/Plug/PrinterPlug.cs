using _Project.Scripts.Player.Interaction;
using _Project.Scripts.Printer;
using _Project.Scripts.Utilities.Events;
using Game.Scripts.Utilities.Events;
using PrimeTween;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace _Project.Scripts.Printer.Plug
{
    public class PrinterPlug : MonoBehaviour, IClickable
    {
        [Header("Plug Settings")]
        [SerializeField] private Transform _plugPosition;
        
        [Header("Wire Settings")]
        [SerializeField] private Transform _endAnchor;
        [SerializeField] private Rigidbody _endAnchorRB;
        [SerializeField] private WireController _wireController;
        
        [Header("Tween Settings")]
        [SerializeField] private Ease _ease = Ease.InOutSine;
        
        [Header("Events")]
        public UnityEvent OnWirePlugged;
        
        private EventBus _eventBus;
        private Sequence _plugSequence;
        private bool _isPlugged;
        private bool _isPlugToggleBlocked;
        
        private static readonly Vector3 UnpluggedPosition = new Vector3(-0.896084368f, 1.52068841f, -1.18789697f);
        private static readonly Vector3 UnpluggedRotation = new Vector3(0, 240f, 0);
        
        private static readonly Vector3 IntermediatePosition1 = new Vector3(-1.04999995f, 1.80700004f, -1.07500005f);
        private static readonly Vector3 IntermediateRotation1 = new Vector3(90f, 0f, 0f);
        
        private static readonly Vector3 IntermediatePosition2 = new Vector3(-1.04999995f, 1.80700004f, -0.85799998f);
        
        private static readonly Vector3 PluggedPosition = new Vector3(-1.08200002f, 1.66460001f, -0.842000008f);
        
        private const float Duration1 = 1.5f;
        private const float Duration2 = 1.5f;
        private const float Duration3 = 1f;

        [Inject]
        public void Construct(EventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void OnBeginHover()
        {
            _eventBus.Publish(new OnInteractableHoverEvent("Plug", true));
        }

        public void OnEndHover()
        {
            _eventBus.Publish(new OnInteractableHoverEvent("Plug", false));
        }

        public void OnClick()
        {
            if (_isPlugToggleBlocked)
            {
                return;
            }

            TogglePlug();
        }

        private void TogglePlug()
        {
            StopCurrentTween();
            
            if (_isPlugged)
            {
                PlayUnplugSequence();
            }
            else
            {
                PlayPlugSequence();
            }
        }

        private void PlayPlugSequence()
        {
            _plugSequence = Sequence.Create()
                .Group(Tween.LocalPosition(_endAnchor, IntermediatePosition1, Duration1, _ease))
                .Group(Tween.LocalRotation(_endAnchor, Quaternion.Euler(IntermediateRotation1), Duration1, _ease))
                .Chain(Tween.LocalPosition(_endAnchor, IntermediatePosition2, Duration2, _ease))
                .Chain(Tween.LocalPosition(_endAnchor, PluggedPosition, Duration3, _ease))
                .OnComplete(() => OnPlugComplete(true));
        }

        private void PlayUnplugSequence()
        {
            DisconnectWire();
            
            _plugSequence = Sequence.Create()
                .Group(Tween.LocalPosition(_endAnchor, IntermediatePosition2, Duration3, _ease))
                .Chain(Tween.LocalPosition(_endAnchor, IntermediatePosition1, Duration2, _ease))
                .Group(Tween.LocalPosition(_endAnchor, UnpluggedPosition, Duration1, _ease))
                .Group(Tween.LocalRotation(_endAnchor, Quaternion.Euler(UnpluggedRotation), Duration1, _ease))
                .OnComplete(() => OnPlugComplete(false));
        }

        private void OnPlugComplete(bool plugged)
        {
            if (plugged)
            {
                ConnectWire();
                OnWirePlugged?.Invoke();
            }
            
            _eventBus.Publish(new OnInteractableStateChangedEvent<PrinterElement>(PrinterElement.Plug, plugged));
            _eventBus.Publish(new OnPlugStateChanged { IsPlugged = plugged });
        }

        private void ConnectWire()
        {
            if (_endAnchor != null && _endAnchorRB != null && _plugPosition != null)
            {
                _endAnchorRB.isKinematic = true;
                _endAnchor.position = _plugPosition.position;
                _endAnchor.rotation = transform.rotation;
            }
            
            _isPlugged = true;
        }

        private void DisconnectWire()
        {
            _isPlugged = false;
        }

        private void Update()
        {
            if (_isPlugged && _endAnchor != null && _plugPosition != null)
            {
                _endAnchorRB.isKinematic = true;
                _endAnchor.position = _plugPosition.position;
                Vector3 eulerRotation = new Vector3(transform.eulerAngles.x + 90f, transform.eulerAngles.y, transform.eulerAngles.z);
                _endAnchor.rotation = Quaternion.Euler(eulerRotation);
            }
        }

        private void StopCurrentTween()
        {
            if (_plugSequence.isAlive)
            {
                _plugSequence.Stop();
            }
        }

        private void OnDestroy()
        {
            StopCurrentTween();
        }

        private void OnEnable()
        {
            _eventBus?.Subscribe<OnPrintSafetyLockChanged>(OnPrintSafetyLockChanged);
        }

        private void OnDisable()
        {
            _eventBus?.Unsubscribe<OnPrintSafetyLockChanged>(OnPrintSafetyLockChanged);
        }

        private void Start()
        {
            _eventBus.Publish(new OnPlugStateChanged { IsPlugged = _isPlugged });
        }

        private void OnPrintSafetyLockChanged(OnPrintSafetyLockChanged evt)
        {
            _isPlugToggleBlocked = evt.IsPlugToggleBlocked;
        }
    }
}