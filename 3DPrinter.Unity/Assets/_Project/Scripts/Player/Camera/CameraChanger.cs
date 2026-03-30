using Unity.Cinemachine;
using UnityEngine;

namespace _Project.Scripts.Player.Camera
{
    public class CameraChanger : MonoBehaviour
    {
        private const int InactiveCameraPriority = 1;
        private const int ActiveCameraPriority = 2;
        
        [SerializeField] private CameraChangerData _setupChangerData;
        
        private CinemachineCamera _currentCamera;

        public bool IsOnMainCamera()
        {
            if (_currentCamera == null)
                return false;
            
            return _currentCamera == _setupChangerData.StartCamera;
        }
        
        public void ChangeCamera(CinemachineCamera newCamera)
        {
            if (!newCamera)
            {
                Debug.LogError("[CameraChanger.ChangeCamera] Trying to change camera to null");
                return;
            }
            
            if (_currentCamera != null)
            {
                _currentCamera.Priority = InactiveCameraPriority;
            }
            
            _currentCamera = newCamera;
            _currentCamera.Priority = ActiveCameraPriority;
        }

        public void RestoreCamera()
        {
            var startCamera = _setupChangerData.StartCamera;
            
            if (!startCamera)
            {
                Debug.LogError("[CameraChanger.RestoreCamera] Trying to restore camera to null");
                return;
            }

            if (startCamera.TryGetComponent<CinemachineOrbitalFollow>(out var follow))
            {
                var axisData = _setupChangerData.AxisData;
            
                follow.HorizontalAxis.Value = axisData.x;
                follow.VerticalAxis.Value = axisData.y;
                follow.RadialAxis.Value = axisData.z;
            }
            else
            {
                Debug.LogWarning("[CameraChanger.RestoreCamera] No orbital follow component found on start camera");
                return;
            }

            var focusPoint = _setupChangerData.CameraFocusPoint;
            if (focusPoint)
            {
                focusPoint.position = _setupChangerData.FocusPointStartPosition;
            }
            else
            {
                Debug.LogWarning("[CameraChanger.RestoreCamera] No focus point found");
            }
            
            ChangeCamera(startCamera);
        }
    }
}