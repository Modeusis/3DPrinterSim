using UnityEngine;

namespace _Project.Scripts.Printer.Visual
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshRenderer))]
    public class ShaderGraphClipper : MonoBehaviour
    {
        private static readonly int CuttingPlane = Shader.PropertyToID("_CuttingPlane");
        
        [SerializeField] private Transform _cuttingPlane;
        
        private MeshRenderer _meshRenderer;
        private MaterialPropertyBlock _propBlock;

        private void OnEnable()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _propBlock = new MaterialPropertyBlock();
        }

        public void SetCuttingPlane(Transform cuttingPlane)
        {
            _cuttingPlane = cuttingPlane;
        }

        private void Update()
        {
            if (!_cuttingPlane || _meshRenderer == null)
                return;
            
            if (_propBlock == null)
                _propBlock = new MaterialPropertyBlock();

            var normal = _cuttingPlane.up;
            var planePos = _cuttingPlane.position;

            var distance = -Vector3.Dot(normal, planePos);
            var planeData = new Vector4(normal.x, normal.y, normal.z, distance);
            
            _meshRenderer.GetPropertyBlock(_propBlock);
            _propBlock.SetVector(CuttingPlane, planeData);
            _meshRenderer.SetPropertyBlock(_propBlock);
        }
    }
}