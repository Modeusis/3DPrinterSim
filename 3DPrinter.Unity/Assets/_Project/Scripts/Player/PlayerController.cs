using _Project.Scripts.Player.Camera;
using _Project.Scripts.Player.Interaction;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Camera Setup")]
        [SerializeField] private CameraData _cameraData;
        [SerializeField] private Transform _cameraTarget;
        
        private BaseInput _baseInput;
        
        private CameraController _cameraController;
        private InteractionController _interactionController;
        
        private bool _isInitialized;
        
        private void Awake()
        {
            _baseInput = new BaseInput();
            _baseInput.Enable();
            
            InitializeCamera();
            InitializeInput();
            
            _isInitialized = true;
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

        private void InitializeInput()
        {
            _interactionController = new InteractionController(_baseInput, UnityEngine.Camera.main);
        }
        
        private void Update()
        {
            if (!_isInitialized)
            {
                return;
            }
            
            _cameraController.Update();
            _interactionController.Update();
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