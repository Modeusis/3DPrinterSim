using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.UI
{
    public class ListShowable : MonoBehaviour
    {
        [SerializeField] private List<Showable> _showables;

        private bool _isOpen;
        
        public void Toggle()
        {
            if (!_isOpen)
            {
                ShowAll();
            }
            else
            {
                HideAll();
            }
        }
        
        public void ShowAll()
        {
            if (_showables.Count == 0)
                return;

            foreach (var showable in _showables)
            {
                showable.Show();
            }
            
            _isOpen = true;
        }
        
        public void HideAll()
        {
            if (_showables.Count == 0)
                return;

            foreach (var showable in _showables)
            {
                showable.Hide();
            }
            
            _isOpen = false;
        }
    }
}