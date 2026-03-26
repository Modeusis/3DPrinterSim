using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Player.Camera
{
    public class CameraController
    {
        private CameraData _data;
        private Transform _cameraTarget;
        private BaseInput _baseInput;

        private InputAction _zoomAction;
        private InputAction _moveAction;
        private InputAction _rotateAction;
        private InputAction _rightClickAction;
    
        private bool _isInitialized;
    
        public void Init(CameraData data, Transform cameraTarget, BaseInput baseInput)
        {
            _data = data;
            _cameraTarget = cameraTarget;
            _baseInput = baseInput;
        
            if (_data == null)
            {
                Debug.LogError("[CameraController.Init] CameraData is null, check inspector");
                return;
            }
        
            if (_data.VirtualCamera == null)
            {
                Debug.LogError("[CameraController.Init] VirtualCamera is null in CameraData, check inspector");
                return;
            }
        
            if (_data.Follow == null)
            {
                Debug.LogError("[CameraController.Init] Follow component is null in CameraData, check inspector");
                return;
            }
        
            if (_cameraTarget == null)
            {
                Debug.LogError("[CameraController.Init] Camera target transform is null");
                return;
            }
        
            if (_baseInput == null)
            {
                Debug.LogError("[CameraController.Init] BaseInput is null");
                return;
            }

            MapActions();

            _isInitialized = true;
        }

        private void MapActions()
        {
            _moveAction = _baseInput.Game.Move;
            _rotateAction = _baseInput.Game.Rotate;
            _zoomAction = _baseInput.Game.Zoom;
            _rightClickAction = _baseInput.Game.RightClick;
        }

        public void Update()
        {
            if (!_isInitialized)
                return;
        
            HandleMovement();
            HandleRotation();
            HandleZoom();
        }

        private void HandleMovement()
        {
            if (_baseInput == null)
                return;
        
            var input = _moveAction.ReadValue<Vector2>();
        
            if (input.sqrMagnitude < 0.01f)
                return;
        
            var moveDir = (Vector3.up * input.y + Vector3.forward * input.x).normalized;
            _cameraTarget.Translate(moveDir * (_data.MoveSpeed * Time.deltaTime), Space.World);
        
            var pos = _cameraTarget.position;
            pos.y = Mathf.Clamp(pos.y, _data.MinY, _data.MaxY);
            pos.z = Mathf.Clamp(pos.z, _data.MinZ, _data.MaxZ);
            _cameraTarget.position = pos;
        }

        private void HandleRotation()
        {
            if (!_rightClickAction.IsPressed())
            {
                return;
            }
        
            var lookInput = _rotateAction.ReadValue<Vector2>();
            _data.Follow.HorizontalAxis.Value += lookInput.x * _data.RotationSpeed * Time.deltaTime;
        }

        private void HandleZoom()
        {
            if (_zoomAction == null || _data.Follow == null)
                return;
        
            var scroll = _zoomAction.ReadValue<float>();

            if (!(Mathf.Abs(scroll) > 0.01f))
                return;
        
            _data.Follow.RadialAxis.Value -= scroll * _data.ZoomSpeed;
            _data.Follow.RadialAxis.Value = Mathf.Clamp(_data.Follow.RadialAxis.Value, _data.MinZoom, _data.MaxZoom);
        }
    
        public void Cleanup()
        {
            _isInitialized = false;
        }
    }
}