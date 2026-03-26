using _Project.Scripts.Player.Interaction;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Player.Interaction
{
    public class InteractionController
    {
        private readonly UnityEngine.Camera _camera;
        
        private readonly InputAction _leftClick;
        private readonly InputAction _mousePosition;
        
        private IClickable _currentClickable;

        public InteractionController(BaseInput input, UnityEngine.Camera camera)
        {
            _camera = camera;

            _leftClick = input.Game.Click;
            _mousePosition = input.Game.MousePosition;
        }
        public void Update()
        {
            HandleRaycast();
            HandleInput();
        }
        
        private void HandleRaycast()
        {
            var ray = _camera.ScreenPointToRay(_mousePosition.ReadValue<Vector2>());
            IClickable newClickable = null;

            if (Physics.Raycast(ray, out var hit)) 
            {
                hit.collider.TryGetComponent(out newClickable);
            }

            if (_currentClickable == newClickable)
                return;
            
            _currentClickable?.OnEndHover();
            _currentClickable = newClickable;
            _currentClickable?.OnBeginHover();
        }

        private void HandleInput()
        {
            if (_leftClick.WasPerformedThisFrame())
            {
                _currentClickable?.OnClick();
            }
        }
    }
}