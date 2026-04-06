using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Printer.Elements
{
    public class Highlighter : MonoBehaviour
    {
        [SerializeField] private Color _highlightTint = Color.yellow;
        
        private Dictionary<Material, Color> _materialsData = new ();
        private bool _isHighlighted;
        
        private void Awake()
        {
            var mesh = GetComponent<MeshRenderer>();
        }

        public void Highlight()
        {
            _materialsData.Clear();

            var mesh = GetComponent<MeshRenderer>();
            if (mesh == null)
            {
                return;
            }

            foreach (var material in mesh.materials)
            {
                _materialsData[material] = material.color;
            }

            foreach (var material in _materialsData.Keys)
            {
                material.color = _highlightTint;
            }

            _isHighlighted = true;
        }

        public void Restore()
        {
            if (!_isHighlighted)
            {
                return;
            }

            foreach (var material in _materialsData.Keys)
            {
                material.color = _materialsData[material];
            }

            _isHighlighted = false;
        }
    }
}