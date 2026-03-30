using UnityEngine;

namespace _Project.Scripts.Printer.Elements
{
    public class DirectHighlighter : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Material _targetMaterial;
        [SerializeField] private Color _highlightTint = Color.yellow;
        
        private Color _originalColor;
        
        private void Awake()
        {
            if (_targetMaterial != null)
            {
                _originalColor = _targetMaterial.color;
            }
            else
            {
                Debug.LogWarning($"[DirectHighlighter] Target material is not set on object {gameObject.name}!");
            }
        }

        public void Highlight()
        {
            if (_targetMaterial != null)
            {
                _targetMaterial.color = _highlightTint;
            }
        }

        public void Restore()
        {
            if (_targetMaterial != null)
            {
                _targetMaterial.color = _originalColor;
            }
        }
    }
}