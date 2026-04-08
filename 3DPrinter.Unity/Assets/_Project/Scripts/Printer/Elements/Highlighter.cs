using System;
using UnityEngine;

namespace _Project.Scripts.Printer.Elements
{
    public class Highlighter : MonoBehaviour
    {
        [SerializeField] private Renderer _targetRenderer;
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
                Debug.LogWarning($"[Highlighter] Renderer is not set on object {gameObject.name}!");
                return;
            }

            if (_highlightMaterial == null)
            {
                Debug.LogWarning($"[Highlighter] Highlight material is not set on object {gameObject.name}!");
                return;
            }

            _originalMaterials = _targetRenderer.sharedMaterials;
            var highlightedMaterials = new Material[_originalMaterials.Length];
            _runtimeHighlightMaterial = new Material(_highlightMaterial);

            ApplyColor(_runtimeHighlightMaterial, highlightColor);
            Array.Fill(highlightedMaterials, _runtimeHighlightMaterial);
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