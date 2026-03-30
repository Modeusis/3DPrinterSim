using System;
using Unity.Cinemachine;
using UnityEngine;

namespace _Project.Scripts.Player.Camera
{
    [Serializable]
    public class CameraChangerData
    {
        [field: SerializeField] public CinemachineCamera StartCamera { get; private set; }
        
        [Tooltip("Determine Axis data for camera: X - Horizontal, Y - Vertical, Z - Radial")]
        [field: SerializeField] public Vector3 AxisData { get; private set; } = new Vector3(-90f, 17.5f, 1f);
        
        [field: Space]
        [field: SerializeField] public Transform CameraFocusPoint { get; private set; }
        [field: SerializeField] public Vector3 FocusPointStartPosition { get; private set; }
    }
}