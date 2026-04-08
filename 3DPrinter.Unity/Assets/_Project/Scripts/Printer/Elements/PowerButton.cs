using _Project.Scripts.Interactables;
using _Project.Scripts.UI.Tasks;
using _Project.Scripts.Utilities.Events;
using Game.Scripts.Utilities.Events;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Printer.Elements
{
    public class PowerButton : InteractableBase<PrinterElement>
    {
        private EventBus _eventBus;
        private TaskManager _taskManager;
        private bool _isPowerToggleBlocked;

        protected override string AnimatorParameterName => "IsActive";
        protected override PrinterElement StateElement => PrinterElement.PowerButton;

        [Inject]
        public void ConstructLocal(EventBus eventBus, [InjectOptional] TaskManager taskManager = null)
        {
            _eventBus = eventBus;
            _taskManager = taskManager;
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
                Debug.LogWarning("[PowerButton.OnClick] Нельзя выключать питание во время печати.");
                _taskManager?.ShowMessage("Нельзя выключать питание во время печати.");
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