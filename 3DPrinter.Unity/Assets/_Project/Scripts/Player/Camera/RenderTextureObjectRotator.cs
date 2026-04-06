using UnityEngine;

namespace _Project.Scripts.Player.Camera
{
    public class RenderTextureObjectRotator : MonoBehaviour
    {
        [SerializeField] private Vector3 _rotationAxis = Vector3.up;
        [SerializeField] private float _rotationSpeed = 30f;
        [SerializeField] private Space _rotationSpace = Space.Self;

        private void Update()
        {
            var axis = _rotationAxis.sqrMagnitude > 0f ? _rotationAxis.normalized : Vector3.up;
            var angle = _rotationSpeed * Time.deltaTime;
            transform.Rotate(axis, angle, _rotationSpace);
        }
    }
}
