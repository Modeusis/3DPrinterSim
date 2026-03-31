using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using _Project.Scripts.Printer.Core;
using _Project.Scripts.Setups.Tooltip;
using PrimeTween;
using TMPro;
using UnityEngine;

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
        [Tooltip("Базовая температура для минимальной скорости")]
        [SerializeField] private float _baseTemperature = 200f;
        
        [Tooltip("На сколько градусов нужно повышать температуру на каждую единицу скорости")]
        [SerializeField] private float _speedToTempMultiplier = 0.15f;
        
        [Tooltip("Делитель для расчета потери качества от скорости и сложности (чем больше, тем меньше штраф)")]
        [SerializeField] private float _accuracyDivisor = 5000f;
        
        [Tooltip("Сколько процентов качества отнимается за каждый градус ошибки от идеальной температуры")]
        [SerializeField] private float _tempErrorPenalty = 2.0f;

        private Complexity _lastComplexity = new Complexity();
        private float _lastSpeed;
        private bool _isFilled;

        private Tween _fadeTween;
        
        private bool _isShowed;
        
        public void Awake()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            Clear();
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
            var temperatureInput = _temperatureInputField.text;
            
            if (_isFilled)
            {
                Debug.LogWarning("[TableController.FillRow] Таблица уже заполнена!");
                return;
            }

            if (!int.TryParse(temperatureInput, out var temperature))
            {
                Debug.LogWarning("[TableController.FillRow] Некорректный ввод температуры, отмена");
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

            foreach (var row in _tableRows.Where(row => !row.IsFilled))
            {
                row.Fill(newData);
                break;
            }

            var allRowsFilled = _tableRows.All(row => row.IsFilled);

            if (!allRowsFilled)
                return;
            
            CalculateAverage();
            _isFilled = true;
        }

        private float GetAccuracy(int inputTemperature)
        {
            var idealTemp = _baseTemperature + (_lastSpeed * _speedToTempMultiplier);
            var speedComplexityPenalty = (_lastSpeed * _lastComplexity.Value) / _accuracyDivisor;
            var tempDeviation = Mathf.Abs(idealTemp - inputTemperature);
            var tempPenalty = tempDeviation * _tempErrorPenalty;
            var finalAccuracy = 100f - speedComplexityPenalty - tempPenalty;
            
            finalAccuracy = Mathf.Clamp(finalAccuracy, 0f, 100f);
            return (float)Math.Round(finalAccuracy, 1);
        }
        
        private void CalculateAverage()
        {
            if (_tableRows.Count == 0) return;

            float totalComplexity = 0f;
            float totalSpeed = 0f;
            float totalTemperature = 0f;
            float totalAccuracy = 0f;

            foreach (var row in _tableRows)
            {
                totalComplexity += row.CachedData.Complexity.Value;
                totalSpeed += row.CachedData.Speed;
                totalTemperature += row.CachedData.Temperature;
                totalAccuracy += row.CachedData.Accuracy;
            }

            int count = _tableRows.Count;
            
            var averageData = new ResearchData
            {
                Complexity = new Complexity { Type = ModelType.Avg, Value = (float)Math.Round(totalComplexity / count, 1) }, 
                Speed = (float)Math.Round(totalSpeed / count, 1),
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
            
            ComplexityField.text = $"{data.Complexity.Type} / {data.Complexity.Value}";
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
    }
}