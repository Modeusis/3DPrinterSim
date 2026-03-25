using _Project.Scripts.Player.Camera;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Camera Setup")]
        [SerializeField] private CameraData _cameraData;
        [SerializeField] private Transform _cameraTarget;
        
        private CameraController _cameraController;
        private BaseInput _baseInput;
        
        private void Awake()
        {
            _baseInput = new BaseInput();
            _baseInput.Enable();
            
            InitializeCamera();
        }
        
        private void InitializeCamera()
        {
            if (_cameraData == null)
            {
                Debug.LogError("[PlayerController] CameraData is not assigned in inspector!");
                return;
            }
            
            if (_cameraTarget == null)
            {
                Debug.LogWarning("[PlayerController] Camera target not assigned, using this transform");
                _cameraTarget = transform;
            }
            
            _cameraController = new CameraController();
            _cameraController.Init(_cameraData, _cameraTarget, _baseInput);
        }
        
        private void Update()
        {
            if (_cameraController != null)
            {
                _cameraController.UpdateCamera();
            }
        }
        
        private void OnDestroy()
        {
            if (_cameraController != null)
            {
                _cameraController.Cleanup();
            }
        }
    }
}