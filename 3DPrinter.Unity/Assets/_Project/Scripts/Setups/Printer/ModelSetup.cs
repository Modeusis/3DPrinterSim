using _Project.Scripts.Printer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Project.Scripts.Setups.Printer
{
    [CreateAssetMenu(fileName = "New Model setup", menuName = "Setup/Model Data")]
    public class ModelSetup : ScriptableObject
    {
        [field: SerializeField] public GameObject Prefab { get; set; }
        [field: SerializeField] public Sprite Screenshot { get; private set; }
        [field: SerializeField] public Complexity Complexity { get; private set; }
        [field: SerializeField] public float TargetHeatPanelHeight { get; private set; }
        [SerializeField] private List<PrintLayerSetup> _layers = new List<PrintLayerSetup>();
        [SerializeField] private List<TemperatureBySpeedSetup> _temperatureBySpeed = new List<TemperatureBySpeedSetup>();
        [SerializeField] private int _defaultTemperature = 200;

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

        public int GetTemperature(SpeedType speedType)
        {
            var profile = _temperatureBySpeed.FirstOrDefault(x => x.SpeedType == speedType);
            return profile != null ? profile.Temperature : _defaultTemperature;
        }
    }

    [Serializable]
    public class PrintLayerSetup
    {
        [field: SerializeField] public float Z { get; private set; }
        [field: SerializeField] public List<Vector2> Points { get; private set; }
    }

    [Serializable]
    public class TemperatureBySpeedSetup
    {
        [field: SerializeField] public SpeedType SpeedType { get; private set; }
        [field: SerializeField] public int Temperature { get; private set; }
    }
}