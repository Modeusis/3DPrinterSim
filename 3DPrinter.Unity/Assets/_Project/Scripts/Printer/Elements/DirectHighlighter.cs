using UnityEngine;

namespace _Project.Scripts.Printer.Elements
{
    public class DirectHighlighter : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Renderer _targetRenderer;
        [SerializeField] private Material _targetMaterial;
        [SerializeField] private Material _highlightMaterial;
        [SerializeField] private Color _defaultHighlightColor = Color.yellow;
        [SerializeField] private Color _alternativeHighlightColor = Color.cyan;

        private Material[] _originalMaterials;
        private Material _runtimeHighlightMaterial;
        private bool _isHighlighted;

        private void Awake()
        {
            if (_targetRenderer == null)
            {
                _targetRenderer = GetComponent<Renderer>();
            }

            if (_targetMaterial == null)
            {
                Debug.LogWarning($"[DirectHighlighter] Target material is not set on object {gameObject.name}!");
            }
        }

        public void Highlight()
        {
            Highlight(_defaultHighlightColor);
        }

        public void AlternativeHighlight()
        {
            Highlight(_alternativeHighlightColor);
        }

        public void Highlight(Color highlightColor)
        {
            if (_isHighlighted)
            {
                UpdateHighlightColor(highlightColor);
                return;
            }

            if (_targetRenderer == null)
            {
                Debug.LogWarning($"[DirectHighlighter] Renderer is not set on object {gameObject.name}!");
                return;
            }

            if (_targetMaterial == null)
            {
                Debug.LogWarning($"[DirectHighlighter] Target material is not set on object {gameObject.name}!");
                return;
            }

            if (_highlightMaterial == null)
            {
                Debug.LogWarning($"[DirectHighlighter] Highlight material is not set on object {gameObject.name}!");
                return;
            }

            var currentMaterials = _targetRenderer.sharedMaterials;
            var highlightedMaterials = new Material[currentMaterials.Length];
            var replacedAny = false;
            _runtimeHighlightMaterial = new Material(_highlightMaterial);

            ApplyColor(_runtimeHighlightMaterial, highlightColor);

            for (var i = 0; i < currentMaterials.Length; i++)
            {
                if (currentMaterials[i] == _targetMaterial)
                {
                    highlightedMaterials[i] = _runtimeHighlightMaterial;
                    replacedAny = true;
                }
                else
                {
                    highlightedMaterials[i] = currentMaterials[i];
                }
            }

            if (!replacedAny)
            {
                ReleaseRuntimeMaterial();
                Debug.LogWarning(
                    $"[DirectHighlighter] Target material was not found on renderer {gameObject.name}!");
                return;
            }

            _originalMaterials = currentMaterials;
            _targetRenderer.sharedMaterials = highlightedMaterials;
            _isHighlighted = true;
        }

        public void Restore()
        {
            if (!_isHighlighted || _targetRenderer == null || _originalMaterials == null)
            {
                return;
            }

            _targetRenderer.sharedMaterials = _originalMaterials;
            _originalMaterials = null;
            ReleaseRuntimeMaterial();
            _isHighlighted = false;
        }

        [ContextMenu("Highlight With Default Color")]
        private void HighlightWithDefaultColorFromContextMenu()
        {
            Highlight();
        }

        private void UpdateHighlightColor(Color highlightColor)
        {
            if (_runtimeHighlightMaterial == null)
            {
                return;
            }

            ApplyColor(_runtimeHighlightMaterial, highlightColor);
        }

        private static void ApplyColor(Material material, Color highlightColor)
        {
            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", highlightColor);
            }

            if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", highlightColor);
            }
        }

        private void ReleaseRuntimeMaterial()
        {
            if (_runtimeHighlightMaterial == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(_runtimeHighlightMaterial);
            }
            else
            {
                DestroyImmediate(_runtimeHighlightMaterial);
            }

            _runtimeHighlightMaterial = null;
        }
    }
}