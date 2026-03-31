using _Project.Scripts.Printer.Core;
using UnityEngine;

namespace _Project.Scripts.Setups.Printer
{
    [CreateAssetMenu(fileName = "New Model setup", menuName = "Setup/Model Data")]
    public class ModelSetup : ScriptableObject
    {
        [field: SerializeField] public GameObject Prefab { get; set; }
        [field: SerializeField] public Complexity Complexity { get; private set; }
        [field: SerializeField] public float TargetHeatPanelHeight { get; private set; }
    }
}