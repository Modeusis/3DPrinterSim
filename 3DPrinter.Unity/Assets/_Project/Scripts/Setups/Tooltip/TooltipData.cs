using UnityEngine;

namespace _Project.Scripts.Setups.Tooltip
{
    [CreateAssetMenu(fileName = "New Tooltip", menuName = "Setup/Tooltip")]
    public class TooltipData : ScriptableObject
    {
        [field: SerializeField] public string Text { get; set; } = "Input tooltip here";
    }
}