using UnityEngine;

namespace _Project.Scripts.Setups.Animation
{
    public class AnimationProperty<T> : ScriptableObject
    {
        [field: SerializeField] public float Duration { get; private set; } = 0.2f;
        [field: SerializeField] public T Value{ get; private set; }
        [field: SerializeField] public AnimationCurve Ease { get; private set; }
    }
}