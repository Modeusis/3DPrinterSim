using System;
using System.Threading;
using _Project.Scripts.Setups.Printer;
using _Project.Scripts.Utilities.Events;
using _Project.Scripts.Printer.Visual;
using _Project.Scripts.Printer.Filament;
using Cysharp.Threading.Tasks;
using Game.Scripts.Utilities.Events;
using UnityEngine;
using Zenject;
using System.Collections.Generic;

namespace _Project.Scripts.Printer.Core
{
    public class PrintProcessController : MonoBehaviour
    {
        [Header("Core references")]
        [SerializeField] private HeatPanelController _heatPanelController;
        [SerializeField] private PrinterHeadController _printerHeadController;
        [SerializeField] private FilamentPlacer _filamentPlacer;
        [SerializeField] private Transform _printedModelRoot;
        [SerializeField] private Transform _clippingPlane;

        [Header("Printed model tween")]
        [SerializeField] private float _pickupTweenDuration = 0.6f;
        [SerializeField] private float _pickupMoveY = 0.5f;
        [SerializeField] private float _pickupScale = 0.2f;

        [Header("Cooling fans")]
        [SerializeField] private List<FanRotationSettings> _fans = new List<FanRotationSettings>();

        private EventBus _eventBus;
        private CancellationTokenSource _printCts;
        private bool _awaitingModelPickup;
        private GameObject _currentPrintedModel;
        private PrintedModelCollectable _currentCollectable;
        private Color _selectedPrintColor = Color.white;

        public bool IsPrinting { get; private set; }
        public bool CanStartNextPrint => !IsPrinting && !_awaitingModelPickup;

        [Inject]
        public void Construct(EventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public async UniTask<bool> StartPrintAsync(ModelSetup modelSetup, SpeedType speedType)
        {
            if (IsPrinting || _awaitingModelPickup || modelSetup == null)
            {
                return false;
            }

            var totalPoints = modelSetup.GetTotalPointsCount();
            if (totalPoints <= 0)
            {
                Debug.LogWarning("[PrintProcessController.StartPrintAsync] Model has no print points.");
                return false;
            }

            IsPrinting = true;
            _printCts = new CancellationTokenSource();
            var token = _printCts.Token;

            _eventBus.Publish(new OnPrintSafetyLockChanged(true, true, true));
            _eventBus.Publish(new OnPrintProcessStarted(modelSetup, speedType));
            _eventBus.Publish(new OnPrintProgressChanged(0f));

            try
            {
                _filamentPlacer?.BeginRotation();
                await _heatPanelController.MoveToHeightAsync(modelSetup.TargetHeatPanelHeight, token);
                SpawnPrintedModel(modelSetup, false);

                var completedPoints = 0;
                var baseTemperature = modelSetup.GetTemperature(speedType);

                foreach (var layer in modelSetup.Layers)
                {
                    await _heatPanelController.MoveToHeightAsync(layer.Z, token);

                    if (layer?.Points == null)
                    {
                        continue;
                    }

                    foreach (var point in layer.Points)
                    {
                        await _printerHeadController.MoveToPointAsync(point, speedType, token);
                        completedPoints++;

                        var temperature = baseTemperature + UnityEngine.Random.Range(-2, 3);
                        var progress = Mathf.Clamp01((float)completedPoints / totalPoints) * 100f;

                        _eventBus.Publish(new OnPrintTemperatureChanged(temperature));
                        _eventBus.Publish(new OnPrintProgressChanged(progress));
                    }
                }

                _eventBus.Publish(new OnPrintProcessFinished(modelSetup, speedType));
                if (_currentCollectable != null)
                {
                    _currentCollectable.SetCollectable(true);
                }
                _awaitingModelPickup = true;
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("[PrintProcessController.StartPrintAsync] Print canceled.");
            }
            finally
            {
                _filamentPlacer?.StopRotation();
                await _printerHeadController.MoveToStartAsync(speedType, CancellationToken.None);
                await _heatPanelController.MoveToStartAsync(CancellationToken.None);
                _eventBus.Publish(new OnPrinterPanelCleared());
                _eventBus.Publish(new OnPrintSafetyLockChanged(false, false, false));
                IsPrinting = false;
                _printCts?.Dispose();
                _printCts = null;
            }

            return true;
        }

        public void CancelPrint()
        {
            if (!IsPrinting)
            {
                return;
            }

            _printCts?.Cancel();
        }

        public void SetPrintColor(Color color)
        {
            _selectedPrintColor = color;

            if (_currentPrintedModel != null)
            {
                ApplyColorToRenderers(_currentPrintedModel.GetComponentsInChildren<Renderer>(), _selectedPrintColor);
            }
        }

        private void Update()
        {
            if (!IsPrinting || _fans.Count == 0)
            {
                return;
            }

            foreach (var fanSettings in _fans)
            {
                if (fanSettings == null || fanSettings.Fan == null)
                {
                    continue;
                }
                
                var axis = fanSettings.Axis.sqrMagnitude > 0f
                    ? fanSettings.Axis.normalized
                    : Vector3.forward;
                var angle = fanSettings.Speed * Time.deltaTime;
                fanSettings.Fan.Rotate(axis, angle, Space.Self);
            }
        }

        private void SpawnPrintedModel(ModelSetup modelSetup, bool isCollectable)
        {
            if (modelSetup.Prefab == null || _printedModelRoot == null)
            {
                return;
            }

            if (_currentPrintedModel != null)
            {
                Destroy(_currentPrintedModel);
            }

            _currentPrintedModel = Instantiate(modelSetup.Prefab, _printedModelRoot);
            _currentPrintedModel.transform.localPosition = Vector3.zero;
            ApplyColorToRenderers(_currentPrintedModel.GetComponentsInChildren<Renderer>(), _selectedPrintColor);
            ApplyClippingPlane(_currentPrintedModel);

            _currentCollectable = _currentPrintedModel.GetComponent<PrintedModelCollectable>();
            if (_currentCollectable == null)
            {
                _currentCollectable = _currentPrintedModel.AddComponent<PrintedModelCollectable>();
            }

            _currentCollectable.Init(_eventBus, _pickupTweenDuration, _pickupMoveY, _pickupScale, OnFinishedModelCollectedInternal, isCollectable);
            _eventBus.Publish(new OnFinishedModelSpawned(_currentPrintedModel));
        }

        private void OnFinishedModelCollectedInternal()
        {
            _currentPrintedModel = null;
            _currentCollectable = null;
            _awaitingModelPickup = false;
            _eventBus.Publish(new OnFinishedModelCollected());
            _eventBus.Publish(new OnNextPrintUnlocked());
        }

        private static void ApplyColorToRenderers(Renderer[] renderers, Color color)
        {
            if (renderers == null)
            {
                return;
            }

            foreach (var renderer in renderers)
            {
                if (renderer == null)
                {
                    continue;
                }

                foreach (var material in renderer.materials)
                {
                    if (material == null)
                    {
                        continue;
                    }

                    if (material.HasProperty("_BaseColor"))
                    {
                        material.SetColor("_BaseColor", color);
                    }

                    if (material.HasProperty("_Color"))
                    {
                        material.SetColor("_Color", color);
                    }
                }
            }
        }

        private void ApplyClippingPlane(GameObject modelInstance)
        {
            if (modelInstance == null || _clippingPlane == null)
            {
                return;
            }

            var clippers = modelInstance.GetComponentsInChildren<ShaderGraphClipper>(true);
            foreach (var clipper in clippers)
            {
                if (clipper == null)
                {
                    continue;
                }

                clipper.SetCuttingPlane(_clippingPlane);
            }
        }
    }

    [Serializable]
    public class FanRotationSettings
    {
        [field: SerializeField] public Transform Fan { get; private set; }
        [field: SerializeField] public Vector3 Axis { get; private set; } = Vector3.forward;
        [field: SerializeField] public float Speed { get; private set; } = 720f;
    }
}
