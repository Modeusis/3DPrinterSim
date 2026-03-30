using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Printer.Elements
{
    public class Highlighter : MonoBehaviour
    {
        [SerializeField] private Color _highlightTint;
        
        private Dictionary<Material, Color> _materialsData = new ();
        
        private void Awake()
        {
            var mesh = GetComponent<MeshRenderer>();
            
            foreach (var material in mesh.sharedMaterials)
            {
                _materialsData.Add(material, material.color);
            }
        }

        private void Highlight()
        {
            foreach (var material in _materialsData.Keys)
            {
                material.color = _highlightTint;
            }
        }

        private void Restore()
        {
            foreach (var material in _materialsData.Keys)
            {
                material.color = _materialsData[material];
            }
        }
    }
}