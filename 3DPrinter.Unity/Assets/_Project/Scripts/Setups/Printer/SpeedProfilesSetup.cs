using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Project.Scripts.Setups.Printer
{
    [CreateAssetMenu(fileName = "New speed profiles", menuName = "Setup/Printer speed")]
    public class SpeedProfilesSetup : ScriptableObject
    {
        [SerializeField] private List<SpeedProfile> _speedProfiles;
        [SerializeField] private float _defaultSpeed;
        
        public float GetSpeed(SpeedType type)
        {
            var profile = _speedProfiles.FirstOrDefault(p => p.Type == type);

            if (profile == null)
            {
                return _defaultSpeed;
            }

            return profile.Value;
        }
    }

    [Serializable]
    public class SpeedProfile
    {
        [field: SerializeField] public SpeedType Type { get; private set; }
        [field: SerializeField] public float Value { get; private set; }
    }

    public enum SpeedType
    {
        Low,
        Medium,
        High
    }
}