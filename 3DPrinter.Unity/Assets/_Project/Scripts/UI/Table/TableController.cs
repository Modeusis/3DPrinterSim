using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using _Project.Scripts.Printer.Core;
using _Project.Scripts.Setups.Printer;
using _Project.Scripts.Setups.Tooltip;
using _Project.Scripts.UI.Tasks;
using _Project.Scripts.Utilities.Events;
using Game.Scripts.Utilities.Events;
using PrimeTween;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.UI.Table
{
    [RequireComponent(typeof(CanvasGroup))]
    public class TableController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<TableRow> _tableRows;
        [SerializeField] private TableRow _averageRow;
        [SerializeField] private TMP_InputField _temperatureInputField;
        
        [Space]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _fadeInDuration = 0.2f;
        [SerializeField] private float _fadeOutDuration = 0.2f;
        
        [Header("Simulation Settings")]
        [Tooltip("Сколько процентов качества отнимается за каждый градус ошибки от идеальной температуры")]
        [SerializeField] private float _tempErrorPenalty = 2.0f;
        [SerializeField] private float _speedPenaltyWeight = 8f;
        [SerializeField] private float _complexitySpeedPenaltyWeight = 14f;
        [SerializeField] private float _minSpeedValue = 0.0025f;
        [SerializeField] private float _maxSpeedValue = 0.0075f;
        [SerializeField] private float _minComplexityValue = 70f;
        [SerializeField] private float _maxComplexityValue = 130f;
        [SerializeField] private float _maxAccuracy = 97f;
        [SerializeField] private float _minAccuracy = 0f;
        [SerializeField] private Vector2 _accuracyNoiseRange = new Vector2(0f, 5f);

        private EventBus _eventBus;
        private TaskManager _taskManager;
        private ModelSetup _lastPrintedModel;
        private Complexity _lastComplexity = new Complexity();
        private float _lastSpeed;
        private int _targetTemperature;
        private bool _isFilled;
        private bool _hasCompletedPrint;

        private Tween _fadeTween;
        
        private bool _isShowed;

        [Inject]
        public void Construct(EventBus eventBus, [InjectOptional] TaskManager taskManager = null)
        {
            _eventBus = eventBus;
            _taskManager = taskManager;
        }
        
        public void Awake()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            Clear();
        }

        private void OnEnable()
        {
            if (_eventBus == null)
            {
                return;
            }

            _eventBus.Subscribe<OnPrintProcessStarted>(OnPrintStarted);
            _eventBus.Subscribe<OnPrintProcessFinished>(OnPrintFinished);
        }

        private void OnDisable()
        {
            if (_eventBus == null)
            {
                return;
            }

            _eventBus.Unsubscribe<OnPrintProcessStarted>(OnPrintStarted);
            _eventBus.Unsubscribe<OnPrintProcessFinished>(OnPrintFinished);
        }

        public void SetCurrentPrintParams(Complexity complexity, float speed)
        {
            _lastComplexity = complexity;
            _lastSpeed = speed;
        }

        public void Toggle()
        {
            if (_isShowed)
            {
                Hide();
                return;
            }
            
            Show();
        }
        
        public void Show()
        {
            if (_isShowed)
                return;
            
            _fadeTween.Stop();
            
            _fadeTween = Tween.Alpha(_canvasGroup, 1f, _fadeInDuration)
                .OnComplete(target: _canvasGroup, target =>
                {
                    target.interactable = true;
                    target.blocksRaycasts = true;
                });
            
            _isShowed = true;
        }

        public void Hide()
        {
            if (!_isShowed)
                return;
            
            _fadeTween.Stop();
            
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            _fadeTween = Tween.Alpha(_canvasGroup, 0f, _fadeOutDuration);
            
            _isShowed = false;
        }
        
        public void FillRow()
        {
            var temperatureInput = _temperatureInputField != null ? _temperatureInputField.text : string.Empty;
            
            if (_isFilled)
            {
                Debug.LogWarning("[TableController.FillRow] Таблица уже заполнена!");
                _taskManager?.ShowMessage("Таблица уже заполнена.");
                return;
            }

            if (!_hasCompletedPrint || _lastPrintedModel == null)
            {
                Debug.LogWarning("[TableController.FillRow] Нет завершенной печати для записи в таблицу.");
                _taskManager?.ShowMessage("Сначала напечатайте модель, затем заполняйте таблицу.");
                return;
            }

            if (!int.TryParse(temperatureInput, out var temperature))
            {
                Debug.LogWarning("[TableController.FillRow] Некорректный ввод температуры, отмена");
                _taskManager?.ShowMessage("Введите корректную температуру.");
                return;
            }
            
            var accuracy = GetAccuracy(temperature);

            var newData = new ResearchData
            {
                Complexity = _lastComplexity,
                Speed = _lastSpeed,
                Temperature = temperature,
                Accuracy = accuracy
            };

            var nextRow = _tableRows.FirstOrDefault(row => !row.IsFilled);
            if (nextRow == null)
            {
                Debug.LogWarning("[TableController.FillRow] В таблице больше нет свободных строк.");
                _isFilled = true;
                _taskManager?.ShowMessage("В таблице больше нет свободных строк.");
                return;
            }

            nextRow.Fill(newData);
            _hasCompletedPrint = false;
            _taskManager?.CompleteStep(TaskStepType.FillTable);
            _taskManager?.ClearMessage();

            if (_temperatureInputField != null)
            {
                _temperatureInputField.text = string.Empty;
            }

            var allRowsFilled = _tableRows.All(row => row.IsFilled);

            if (!allRowsFilled)
                return;
            
            CalculateAverage();
            _isFilled = true;
            _taskManager?.CompleteStep(TaskStepType.CompleteSimulation);
        }

        private float GetAccuracy(int inputTemperature)
        {
            var tempDeviation = Mathf.Abs(_targetTemperature - inputTemperature);
            var tempPenalty = tempDeviation * _tempErrorPenalty;
            var normalizedSpeed = Normalize(_lastSpeed, _minSpeedValue, _maxSpeedValue);
            var complexityValue = _lastComplexity != null ? _lastComplexity.Value : 0f;
            var normalizedComplexity = Normalize(complexityValue, _minComplexityValue, _maxComplexityValue);
            var speedPenalty = normalizedSpeed * _speedPenaltyWeight;
            var complexitySpeedPenalty = normalizedComplexity * normalizedSpeed * _complexitySpeedPenaltyWeight;
            var baseAccuracy = _maxAccuracy - tempPenalty - speedPenalty - complexitySpeedPenalty;
            var noiseMin = Mathf.Min(_accuracyNoiseRange.x, _accuracyNoiseRange.y);
            var noiseMax = Mathf.Max(_accuracyNoiseRange.x, _accuracyNoiseRange.y);
            var randomizedPenalty = UnityEngine.Random.Range(noiseMin, noiseMax);
            var finalAccuracy = baseAccuracy - randomizedPenalty;

            finalAccuracy = Mathf.Clamp(finalAccuracy, _minAccuracy, _maxAccuracy);
            return (float)Math.Round(finalAccuracy, 1);
        }
        
        private void CalculateAverage()
        {
            if (_tableRows.Count == 0) return;

            var totalComplexity = 0f;
            var totalSpeed = 0f;
            var totalTemperature = 0f;
            var totalAccuracy = 0f;

            foreach (var row in _tableRows)
            {
                totalComplexity += row.CachedData.Complexity.Value;
                totalSpeed += row.CachedData.Speed;
                totalTemperature += row.CachedData.Temperature;
                totalAccuracy += row.CachedData.Accuracy;
            }

            var count = _tableRows.Count;
            
            var averageData = new ResearchData
            {
                Complexity = new Complexity { Type = ModelType.Avg, Value = (float)Math.Round(totalComplexity / count, 1) }, 
                Speed = (float)Math.Round(totalSpeed / count, 6),
                Temperature = (int)Math.Round(totalTemperature / count),
                Accuracy = (float)Math.Round(totalAccuracy / count, 1)
            };

            _averageRow.Fill(averageData);
        }

        public void Clear()
        {
            foreach (var row in _tableRows)
            {
                row.Clear();
            }
            
            _averageRow.Clear();
            _isFilled = false;
            _hasCompletedPrint = false;
        }

        private void OnPrintStarted(OnPrintProcessStarted evt)
        {
            CachePrintParams(evt.Model, evt.SpeedValue);
            _hasCompletedPrint = false;
        }

        private void OnPrintFinished(OnPrintProcessFinished evt)
        {
            CachePrintParams(evt.Model, evt.SpeedValue);
            _hasCompletedPrint = true;
        }

        private void CachePrintParams(ModelSetup model, float speedValue)
        {
            _lastPrintedModel = model;
            _lastSpeed = speedValue;
            _targetTemperature = model != null ? model.TargetTemperature : 0;
            _lastComplexity = ResolveComplexity(model);
        }

        private static float Normalize(float value, float min, float max)
        {
            if (max <= min)
            {
                return 0f;
            }

            return Mathf.Clamp01((value - min) / (max - min));
        }

        private static Complexity ResolveComplexity(ModelSetup model)
        {
            if (model?.Complexity != null && model.Complexity.Value > 0f)
            {
                return new Complexity
                {
                    Type = model.Complexity.Type,
                    Value = model.Complexity.Value
                };
            }

            return new Complexity
            {
                Type = ModelType.Unknown,
                Value = model != null ? model.GetTotalPointsCount() : 0f
            };
        }
    }

    [Serializable]
    public class TableRow
    {
        [field: SerializeField] public TMP_Text ComplexityField { get; private set; }
        [field: SerializeField] public TMP_Text SpeedField { get; private set; }
        [field: SerializeField] public TMP_Text TemperatureField { get; private set; }
        [field: SerializeField] public TMP_Text AccuracyField { get; private set; }

        public bool IsFilled { get; private set; }
        public ResearchData CachedData { get; private set; }
        
        public void Fill(ResearchData data)
        {
            if (IsFilled)
                return;
            
            CachedData = data;

            ComplexityField.text = FormatComplexity(data.Complexity);
            SpeedField.text = data.Speed.ToString(CultureInfo.CurrentCulture);
            TemperatureField.text = data.Temperature.ToString(CultureInfo.CurrentCulture);
            AccuracyField.text = data.Accuracy.ToString("F1", CultureInfo.InvariantCulture) + "%";
            
            IsFilled = true;
        }

        public void Clear()
        {
            CachedData = null;
            
            ComplexityField.text = string.Empty;
            SpeedField.text = string.Empty;
            TemperatureField.text = string.Empty;
            AccuracyField.text = string.Empty;
            
            IsFilled = false;
        }

        private static string FormatComplexity(Complexity complexity)
        {
            if (complexity == null)
            {
                return string.Empty;
            }

            if (complexity.Type == ModelType.Avg)
            {
                return $"AVG / {complexity.Value}";
            }

            if (complexity.Type == ModelType.Unknown || complexity.Type == ModelType.None)
            {
                return complexity.Value.ToString(CultureInfo.CurrentCulture);
            }

            return $"{complexity.Type} / {complexity.Value}";
        }
    }
}