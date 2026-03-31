using UnityEngine;

namespace _Project.Scripts.Printer.Visual
{
    public class ShaderGraphClipper : MonoBehaviour
    {
        private static readonly int CuttingPlane = Shader.PropertyToID("_CuttingPlane");
        
        [SerializeField] private Transform _cuttingPlane;
        private Material _material;

        private void Start()
        {
            _material = GetComponent<MeshRenderer>().material;
        }

        private void Update()
        {
            if (!_cuttingPlane)
                return;

            var normal = _cuttingPlane.up;
            var planePos = _cuttingPlane.position;

            var distance = -Vector3.Dot(normal, planePos);

            var planeData = new Vector4(normal.x, normal.y, normal.z, distance);

            _material.SetVector(CuttingPlane, planeData);
        }
    }
}