using System;
using Unity.Cinemachine;
using UnityEngine;

namespace _Project.Scripts.Player.Camera
{
    [Serializable]
    public class CameraData
    {
        [Header("Cinemachine")]
        [Tooltip("Setup virtual camera")]
        [field: SerializeField] public CinemachineCamera VirtualCamera { get; private set; }
        [field: SerializeField] public CinemachineOrbitalFollow Follow { get; private set; }

        [Header("Speed setup")]
        [field: SerializeField] public float MoveSpeed { get; private set; } = 10f;
        [field: SerializeField] public float RotationSpeed { get; private set; } = 300f;
        [field: SerializeField] public float ZoomSpeed { get; private set; } = 5f;

        [Header("Room Limits")]
        [field: SerializeField] public float MinY { get; private set; } = 1.4f;
        [field: SerializeField] public float MaxY { get; private set; } = 3.5f;
        [field: SerializeField] public float MinZ { get; private set; } = -2f;
        [field: SerializeField] public float MaxZ { get; private set; } = 2f;

        [Header("Zoom Limits")]
        [field: SerializeField] public float MinZoom { get; private set; } = 2f;
        [field: SerializeField] public float MaxZoom { get; private set; } = 20f;
    }
}