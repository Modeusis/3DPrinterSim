using _Project.Scripts.Interactables;
using _Project.Scripts.Utilities.Events;
using Game.Scripts.Utilities.Events;
using Zenject;

namespace _Project.Scripts.Printer.Elements
{
    public class PowerButton : InteractableBase<PrinterElement>
    {
        private EventBus _eventBus;
        private bool _isPowerToggleBlocked;

        protected override string AnimatorParameterName => "IsActive";
        protected override PrinterElement StateElement => PrinterElement.PowerButton;

        [Inject]
        public void ConstructLocal(EventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void OnEnable()
        {
            _eventBus?.Subscribe<OnPrintSafetyLockChanged>(OnPrintSafetyLockChanged);
        }

        private void OnDisable()
        {
            _eventBus?.Unsubscribe<OnPrintSafetyLockChanged>(OnPrintSafetyLockChanged);
        }

        public override void OnClick()
        {
            if (_isPowerToggleBlocked)
            {
                return;
            }

            base.OnClick();
        }

        private void OnPrintSafetyLockChanged(OnPrintSafetyLockChanged evt)
        {
            _isPowerToggleBlocked = evt.IsPowerToggleBlocked;
        }
    }
}