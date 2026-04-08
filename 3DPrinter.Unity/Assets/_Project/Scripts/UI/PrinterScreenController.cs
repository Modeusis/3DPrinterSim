using System.Collections.Generic;
using _Project.Scripts.Printer;
using _Project.Scripts.Printer.Core;
using _Project.Scripts.Setups.Printer;
using _Project.Scripts.UI.Tasks;
using _Project.Scripts.Utilities.Events;
using Cysharp.Threading.Tasks;
using Game.Scripts.Utilities.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI
{
    public class PrinterScreenController : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private CustomButton _leftButton;
        [SerializeField] private CustomButton _rightButton;
        [SerializeField] private CustomButton _startPrintButton;
        [SerializeField] private CustomButton _speedButton;
        [SerializeField] private CustomButton _colorButton;

        [Header("Data")]
        [SerializeField] private List<ModelSetup> _models;
        [SerializeField] private SpeedType _startSpeed = SpeedType.Medium;
        [SerializeField] private List<Color> _availableColors = new List<Color> { Color.white };

        [Header("Core")]
        [SerializeField] private PrintProcessController _printProcessController;
        [SerializeField] private Material _filamentMaterial;
        [SerializeField] private Material _filamentLineMaterial;

        [Header("Pages")]
        [SerializeField] private CanvasGroup _screenCanvasGroup;
        [SerializeField] private GameObject _selectionPage;
        [SerializeField] private GameObject _progressPage;

        [Header("Selection UI")]
        [SerializeField] private Image _modelPreview;
        [SerializeField] private TMP_Text _modelNameText;
        [SerializeField] private TMP_Text _speedText;
        [SerializeField] private TMP_Text _colorText;
        [SerializeField] private Image _colorPreviewImage;

        [Header("Progress UI")]
        [SerializeField] private TMP_Text _temperatureText;
        [SerializeField] private TMP_Text _progressText;

        private EventBus _eventBus;
        private TaskManager _taskManager;
        private int _currentModelIndex;
        private SpeedType _currentSpeed;
        private int _currentColorIndex;
        private bool _isPowered;
        private bool _isFilamentPlaced;

        [Inject]
        public void Construct(EventBus eventBus, [InjectOptional] TaskManager taskManager = null)
        {
            _eventBus = eventBus;
            _taskManager = taskManager;
        }

        private void Awake()
        {
            _currentSpeed = _startSpeed;
            _currentModelIndex = 0;
            _currentColorIndex = 0;

            BindButtons();
            UpdateScreenModelData();
            UpdateSpeedLabel();
            UpdateColor();
            SetDefaultRuntimeLabels();
            SetProgressPageActive(false);
            RefreshScreenPowerAndFilamentState();
            RefreshStartButtonState();
        }

        private void OnEnable()
        {
            _eventBus.Subscribe<OnPrintProgressChanged>(OnPrintProgressChanged);
            _eventBus.Subscribe<OnPrintTemperatureChanged>(OnPrintTemperatureChanged);
            _eventBus.Subscribe<OnPrintProcessStarted>(OnPrintStarted);
            _eventBus.Subscribe<OnNextPrintUnlocked>(OnNextPrintUnlocked);
            _eventBus.Subscribe<OnPrintProcessFinished>(OnPrintFinished);
            _eventBus.Subscribe<OnPowerStateChanged>(OnPowerStateChanged);
            _eventBus.Subscribe<OnFilamentStateChanged>(OnFilamentStateChanged);
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<OnPrintProgressChanged>(OnPrintProgressChanged);
            _eventBus.Unsubscribe<OnPrintTemperatureChanged>(OnPrintTemperatureChanged);
            _eventBus.Unsubscribe<OnPrintProcessStarted>(OnPrintStarted);
            _eventBus.Unsubscribe<OnNextPrintUnlocked>(OnNextPrintUnlocked);
            _eventBus.Unsubscribe<OnPrintProcessFinished>(OnPrintFinished);
            _eventBus.Unsubscribe<OnPowerStateChanged>(OnPowerStateChanged);
            _eventBus.Unsubscribe<OnFilamentStateChanged>(OnFilamentStateChanged);
        }

        private void BindButtons()
        {
            _leftButton.OnClick.AddListener(SelectPreviousModel);
            _rightButton.OnClick.AddListener(SelectNextModel);
            _speedButton.OnClick.AddListener(SelectNextSpeed);
            _colorButton.OnClick.AddListener(SelectNextColor);
            _startPrintButton.OnClick.AddListener(StartPrint);
        }

        private void SelectPreviousModel()
        {
            if (_models.Count == 0)
            {
                Debug.LogWarning("[PrinterScreenController.SelectPreviousModel] Нет доступных моделей для выбора.");
                _taskManager?.ShowMessage("Нет доступных моделей для выбора.");
                return;
            }

            if (_printProcessController.IsPrinting)
            {
                Debug.LogWarning("[PrinterScreenController.SelectPreviousModel] Нельзя менять модель во время печати.");
                _taskManager?.ShowMessage("Нельзя менять модель во время печати.");
                return;
            }

            _currentModelIndex = (_currentModelIndex - 1 + _models.Count) % _models.Count;
            UpdateScreenModelData();
            _taskManager?.CompleteStep(TaskStepType.ChooseModel);
        }

        private void SelectNextModel()
        {
            if (_models.Count == 0)
            {
                Debug.LogWarning("[PrinterScreenController.SelectNextModel] Нет доступных моделей для выбора.");
                _taskManager?.ShowMessage("Нет доступных моделей для выбора.");
                return;
            }

            if (_printProcessController.IsPrinting)
            {
                Debug.LogWarning("[PrinterScreenController.SelectNextModel] Нельзя менять модель во время печати.");
                _taskManager?.ShowMessage("Нельзя менять модель во время печати.");
                return;
            }

            _currentModelIndex = (_currentModelIndex + 1) % _models.Count;
            UpdateScreenModelData();
            _taskManager?.CompleteStep(TaskStepType.ChooseModel);
        }

        private void SelectNextSpeed()
        {
            if (_printProcessController.IsPrinting)
            {
                Debug.LogWarning("[PrinterScreenController.SelectNextSpeed] Нельзя менять скорость во время печати.");
                _taskManager?.ShowMessage("Нельзя менять скорость во время печати.");
                return;
            }

            _currentSpeed = _currentSpeed switch
            {
                SpeedType.Low => SpeedType.Medium,
                SpeedType.Medium => SpeedType.High,
                _ => SpeedType.Low
            };

            UpdateSpeedLabel();
            _taskManager?.CompleteStep(TaskStepType.ChooseSpeed);
        }

        private void StartPrint()
        {
            StartPrintAsync().Forget();
        }

        private async UniTaskVoid StartPrintAsync()
        {
            if (_models.Count == 0)
            {
                Debug.LogWarning("[PrinterScreenController.StartPrintAsync] Невозможно начать печать: нет доступных моделей.");
                _taskManager?.ShowMessage("Невозможно начать печать: нет доступных моделей.");
                return;
            }

            if (!_isPowered)
            {
                Debug.LogWarning("[PrinterScreenController.StartPrintAsync] Невозможно начать печать: принтер выключен.");
                _taskManager?.ShowMessage("Сначала включите принтер.");
                return;
            }

            if (!_isFilamentPlaced)
            {
                Debug.LogWarning("[PrinterScreenController.StartPrintAsync] Невозможно начать печать: филамент не установлен.");
                _taskManager?.ShowMessage("Сначала установите филамент.");
                return;
            }

            if (!_printProcessController.CanStartNextPrint)
            {
                Debug.LogWarning("[PrinterScreenController.StartPrintAsync] Невозможно начать новую печать: предыдущая печать еще не завершена или модель не собрана.");
                _taskManager?.ShowMessage("Нельзя начать новую печать, пока предыдущая не завершена.");
                return;
            }

            var model = _models[_currentModelIndex];
            await _printProcessController.StartPrintAsync(model, _currentSpeed);
            RefreshStartButtonState();
        }

        private void UpdateScreenModelData()
        {
            if (_models.Count == 0)
            {
                _modelNameText.text = "No model";
                _modelPreview.enabled = false;
                return;
            }

            var model = _models[_currentModelIndex];
            _modelNameText.text = model.name;
            _modelPreview.sprite = model.Screenshot;
            _modelPreview.enabled = model.Screenshot != null;
        }

        private void UpdateSpeedLabel()
        {
            _speedText.text = _currentSpeed.ToString();
        }

        private void SelectNextColor()
        {
            if (_printProcessController.IsPrinting)
            {
                Debug.LogWarning("[PrinterScreenController.SelectNextColor] Нельзя менять цвет во время печати.");
                _taskManager?.ShowMessage("Нельзя менять цвет во время печати.");
                return;
            }

            if (_availableColors.Count == 0)
            {
                Debug.LogWarning("[PrinterScreenController.SelectNextColor] Нет доступных цветов.");
                _taskManager?.ShowMessage("Нет доступных цветов.");
                return;
            }

            _currentColorIndex = (_currentColorIndex + 1) % _availableColors.Count;
            UpdateColor();
        }

        private void UpdateColor()
        {
            if (_availableColors.Count == 0)
            {
                return;
            }

            var color = _availableColors[_currentColorIndex];
            _printProcessController.SetPrintColor(color);

            if (_filamentMaterial != null)
            {
                _filamentMaterial.color = color;
                _filamentLineMaterial.color = color;
            }

            if (_colorText != null)
            {
                _colorText.text = $"Color {_currentColorIndex + 1}";
            }

            if (_colorPreviewImage != null)
            {
                _colorPreviewImage.color = color;
            }

            _eventBus.Publish(new OnFilamentColorChanged { Color = color });
        }

        private void SetDefaultRuntimeLabels()
        {
            _temperatureText.text = "-- C";
            _progressText.text = "0%";
        }

        private void RefreshStartButtonState()
        {
            var isReady = _models.Count > 0 && _printProcessController.CanStartNextPrint && _isPowered && _isFilamentPlaced;
            _startPrintButton.SetActiveButton(isReady);
        }

        private void OnPrintProgressChanged(OnPrintProgressChanged evt)
        {
            _progressText.text = $"{evt.ProgressPercent:0}%";
        }

        private void OnPrintTemperatureChanged(OnPrintTemperatureChanged evt)
        {
            _temperatureText.text = $"{evt.Temperature} C";
        }

        private void OnPrintStarted(OnPrintProcessStarted evt)
        {
            _taskManager?.CompleteStep(TaskStepType.StartPrint);
            SetProgressPageActive(true);
            RefreshStartButtonState();
        }

        private void OnNextPrintUnlocked(OnNextPrintUnlocked evt)
        {
            SetProgressPageActive(false);
            RefreshStartButtonState();
        }

        private void OnPrintFinished(OnPrintProcessFinished evt)
        {
            _taskManager?.CompleteStep(TaskStepType.WaitForPrintFinish);
            RefreshStartButtonState();
        }

        private void OnPowerStateChanged(OnPowerStateChanged evt)
        {
            _isPowered = evt.IsOn;
            RefreshScreenPowerAndFilamentState();
            RefreshStartButtonState();
        }

        private void OnFilamentStateChanged(OnFilamentStateChanged evt)
        {
            _isFilamentPlaced = evt.IsPlaced;
            RefreshScreenPowerAndFilamentState();
            RefreshStartButtonState();
        }

        private void RefreshScreenPowerAndFilamentState()
        {
            var isActive = _isPowered;

            if (_screenCanvasGroup == null)
            {
                return;
            }

            _screenCanvasGroup.alpha = isActive ? 1f : 0f;
            _screenCanvasGroup.interactable = isActive;
            _screenCanvasGroup.blocksRaycasts = isActive;
        }

        private void SetProgressPageActive(bool isProgress)
        {
            if (_selectionPage != null)
            {
                _selectionPage.SetActive(!isProgress);
            }

            if (_progressPage != null)
            {
                _progressPage.SetActive(isProgress);
            }
        }
    }
}
