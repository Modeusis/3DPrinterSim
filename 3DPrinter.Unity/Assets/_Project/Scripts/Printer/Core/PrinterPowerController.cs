using _Project.Scripts.Printer;
using _Project.Scripts.Utilities.Events;
using Game.Scripts.Utilities.Events;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Printer.Core
{
    public class PrinterPowerController : MonoBehaviour
    {
        private EventBus _eventBus;

        private bool _isPlugged;
        private bool _isPowerButtonOn;
        private bool _effectivePowerState;

        [Inject]
        public void Construct(EventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void OnEnable()
        {
            _eventBus.Subscribe<OnPlugStateChanged>(OnPlugStateChanged);
            _eventBus.Subscribe<OnInteractableStateChangedEvent<PrinterElement>>(OnPrinterElementStateChanged);
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<OnPlugStateChanged>(OnPlugStateChanged);
            _eventBus.Unsubscribe<OnInteractableStateChangedEvent<PrinterElement>>(OnPrinterElementStateChanged);
        }

        private void Start()
        {
            // Start scene: printer is de-energized until both plug and button are active.
            PublishPowerState(false);
        }

        private void OnPlugStateChanged(OnPlugStateChanged evt)
        {
            _isPlugged = evt.IsPlugged;
            RefreshEffectivePower();
        }

        private void OnPrinterElementStateChanged(OnInteractableStateChangedEvent<PrinterElement> evt)
        {
            if (evt.Element != PrinterElement.PowerButton)
            {
                return;
            }

            _isPowerButtonOn = evt.State;
            RefreshEffectivePower();
        }

        private void RefreshEffectivePower()
        {
            var newState = _isPlugged && _isPowerButtonOn;
            if (newState == _effectivePowerState)
            {
                return;
            }

            PublishPowerState(newState);
        }

        private void PublishPowerState(bool isOn)
        {
            _effectivePowerState = isOn;
            _eventBus.Publish(new OnPowerStateChanged { IsOn = isOn });
        }
    }
}
