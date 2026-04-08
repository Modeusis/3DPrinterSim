using _Project.Scripts.Printer.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Setups.Printer
{
    [CreateAssetMenu(fileName = "New Model setup", menuName = "Setup/Model Data")]
    public class ModelSetup : ScriptableObject
    {
        [field: SerializeField] public GameObject Prefab { get; set; }
        [field: SerializeField] public Sprite Screenshot { get; private set; }
        [field: SerializeField] public Complexity Complexity { get; private set; }
        [field: SerializeField] public int TargetTemperature { get; private set; } = 200;
        [field: SerializeField] public float TargetHeatPanelHeight { get; private set; }
        [SerializeField] private List<PrintLayerSetup> _layers = new List<PrintLayerSetup>();

        public IReadOnlyList<PrintLayerSetup> Layers => _layers;

        public int GetTotalPointsCount()
        {
            var total = 0;

            foreach (var layer in _layers)
            {
                if (layer?.Points == null)
                {
                    continue;
                }

                total += layer.Points.Count;
            }

            return total;
        }
    }

    [Serializable]
    public class PrintLayerSetup
    {
        [field: SerializeField] public float Z { get; private set; }
        [field: SerializeField] public List<Vector2> Points { get; private set; }
    }
}