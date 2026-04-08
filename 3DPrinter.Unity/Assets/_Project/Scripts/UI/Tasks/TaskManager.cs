using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.UI.Tasks
{
    public class TaskManager
    {
        private readonly TMP_Text _currentTaskText;
        private readonly TMP_Text _messageText;
        private readonly CanvasGroup _messageCanvasGroup;
        private readonly float _messageFadeInDuration;
        private readonly float _messageFadeOutDuration;
        private readonly float _messageAutoHideDelay;
        private readonly bool _autoHideMessages;
        private readonly string _allTasksCompletedMessage;
        private readonly List<RuntimeTaskStep> _steps;
        private readonly Dictionary<TaskStepType, List<RuntimeTaskStep>> _stepsByType;

        private Tween _messageFadeTween;
        private Tween _messageHideTween;
        private TaskStepType? _previewStep;

        public TaskManager(
            TaskChainSetup taskChainSetup,
            TMP_Text currentTaskText,
            TMP_Text messageText,
            CanvasGroup messageCanvasGroup,
            float messageFadeInDuration,
            float messageFadeOutDuration,
            float messageAutoHideDelay,
            bool autoHideMessages)
        {
            _currentTaskText = currentTaskText;
            _messageText = messageText;
            _messageCanvasGroup = messageCanvasGroup;
            _messageFadeInDuration = messageFadeInDuration;
            _messageFadeOutDuration = messageFadeOutDuration;
            _messageAutoHideDelay = messageAutoHideDelay;
            _autoHideMessages = autoHideMessages;
            _allTasksCompletedMessage = taskChainSetup != null
                ? taskChainSetup.AllTasksCompletedMessage
                : "All tasks completed.";
            _steps = BuildRuntimeSteps(taskChainSetup);
            _stepsByType = BuildStepsByType(_steps);

            InitializeMessageView();
            ResetTasks();
        }

        public void ResetTasks()
        {
            foreach (var step in _steps)
            {
                step.ResetState();
            }

            _previewStep = null;
            ClearMessage();
            RefreshCurrentTaskText();
        }

        public void CompleteStep(TaskStepType stepType)
        {
            if (!TryGetStepToComplete(stepType, out var step))
            {
                Debug.LogWarning($"[TaskManager.CompleteStep] Step {stepType} was not found in the chain.");
                return;
            }

            step.SetCompleted(true);
            _previewStep = null;
            RefreshCurrentTaskText();
        }

        public void UncompleteStep(TaskStepType stepType)
        {
            if (!TryGetStepToUncomplete(stepType, out var step))
            {
                Debug.LogWarning($"[TaskManager.UncompleteStep] Step {stepType} was not found in the chain.");
                return;
            }

            step.SetCompleted(false);
            _previewStep = null;
            RefreshCurrentTaskText();
        }

        public void SetCurrentStep(TaskStepType stepType)
        {
            if (!TryGetStepToPreview(stepType, out var step))
            {
                Debug.LogWarning($"[TaskManager.SetCurrentStep] Step {stepType} was not found in the chain.");
                return;
            }

            _previewStep = stepType;
            SetTaskText(step.GetDisplayText());
        }

        public bool IsStepCompleted(TaskStepType stepType)
        {
            var steps = _steps.Where(x => x.StepType == stepType).ToList();
            return steps.Count > 0 && steps.All(IsStepCompleted);
        }

        public TaskStepType GetCurrentIncompleteStep()
        {
            var step = GetFirstUncompletedStep();
            return step != null ? step.StepType : TaskStepType.None;
        }

        public void ShowMessage(string message)
        {
            ShowMessage(message, _autoHideMessages);
        }

        public void ShowMessage(string message, bool autoHide)
        {
            if (_messageText == null)
            {
                return;
            }

            _messageFadeTween.Stop();
            _messageHideTween.Stop();
            _messageText.text = message ?? string.Empty;

            if (_messageCanvasGroup == null)
            {
                return;
            }

            _messageFadeTween = Tween.Alpha(_messageCanvasGroup, 1f, _messageFadeInDuration)
                .OnComplete(target: _messageCanvasGroup, target =>
                {
                    target.interactable = true;
                    target.blocksRaycasts = true;
                });

            if (autoHide)
            {
                _messageHideTween = Tween.Alpha(_messageCanvasGroup, 0f, _messageFadeOutDuration, startDelay: _messageAutoHideDelay)
                    .OnComplete(() =>
                    {
                        _messageCanvasGroup.interactable = false;
                        _messageCanvasGroup.blocksRaycasts = false;

                        if (_messageText != null)
                        {
                            _messageText.text = string.Empty;
                        }
                    });
            }
        }

        public void ClearMessage()
        {
            ClearMessage(0f);
        }

        private void RefreshCurrentTaskText()
        {
            if (_previewStep.HasValue && TryGetStepToPreview(_previewStep.Value, out var previewStep))
            {
                SetTaskText(previewStep.GetDisplayText());
                return;
            }

            var currentStep = GetFirstUncompletedStep();
            if (currentStep != null)
            {
                SetTaskText(currentStep.GetDisplayText());
                return;
            }

            SetTaskText(_allTasksCompletedMessage);
        }

        private RuntimeTaskStep GetFirstUncompletedStep()
        {
            return _steps.FirstOrDefault(step => !IsStepCompleted(step));
        }

        private bool TryGetStepToComplete(TaskStepType stepType, out RuntimeTaskStep step)
        {
            step = _steps.FirstOrDefault(x => x.StepType == stepType && !IsStepCompleted(x))
                ?? _steps.FirstOrDefault(x => x.StepType == stepType);
            return step != null;
        }

        private bool TryGetStepToUncomplete(TaskStepType stepType, out RuntimeTaskStep step)
        {
            step = _steps.LastOrDefault(x => x.StepType == stepType && x.IsSelfCompleted)
                ?? _steps.LastOrDefault(x => x.StepType == stepType);
            return step != null;
        }

        private bool TryGetStepToPreview(TaskStepType stepType, out RuntimeTaskStep step)
        {
            step = _steps.FirstOrDefault(x => x.StepType == stepType && !IsStepCompleted(x))
                ?? _steps.FirstOrDefault(x => x.StepType == stepType);
            return step != null;
        }

        private bool IsStepCompleted(RuntimeTaskStep step)
        {
            return IsStepCompleted(step, new HashSet<RuntimeTaskStep>());
        }

        private bool IsStepCompleted(RuntimeTaskStep step, HashSet<RuntimeTaskStep> visitedSteps)
        {
            if (step == null)
            {
                return false;
            }

            if (step.IsSelfCompleted)
            {
                return true;
            }

            if (!visitedSteps.Add(step))
            {
                return false;
            }

            foreach (var relatedStepType in step.CompleteWhenAnyOfCompleted)
            {
                if (!_stepsByType.TryGetValue(relatedStepType, out var relatedSteps))
                {
                    continue;
                }

                if (relatedSteps.Any(relatedStep => IsStepCompleted(relatedStep, visitedSteps)))
                {
                    visitedSteps.Remove(step);
                    return true;
                }
            }

            visitedSteps.Remove(step);
            return false;
        }

        private void ClearMessage(float startDelay)
        {
            if (_messageCanvasGroup == null)
            {
                if (_messageText != null)
                {
                    _messageText.text = string.Empty;
                }

                return;
            }

            _messageFadeTween.Stop();
            _messageHideTween.Stop();
            _messageCanvasGroup.interactable = false;
            _messageCanvasGroup.blocksRaycasts = false;

            _messageFadeTween = Tween.Alpha(_messageCanvasGroup, 0f, _messageFadeOutDuration, startDelay: startDelay)
                .OnComplete(() =>
                {
                    if (_messageText != null)
                    {
                        _messageText.text = string.Empty;
                    }
                });
        }

        private void InitializeMessageView()
        {
            if (_messageCanvasGroup == null)
            {
                return;
            }

            _messageCanvasGroup.alpha = 0f;
            _messageCanvasGroup.interactable = false;
            _messageCanvasGroup.blocksRaycasts = false;
        }

        private static List<RuntimeTaskStep> BuildRuntimeSteps(TaskChainSetup taskChainSetup)
        {
            if (taskChainSetup == null || taskChainSetup.Steps == null)
            {
                return new List<RuntimeTaskStep>();
            }

            return taskChainSetup.Steps
                .Where(step => step != null)
                .Select(step => new RuntimeTaskStep(step))
                .ToList();
        }

        private static Dictionary<TaskStepType, List<RuntimeTaskStep>> BuildStepsByType(IEnumerable<RuntimeTaskStep> steps)
        {
            return steps
                .GroupBy(step => step.StepType)
                .ToDictionary(group => group.Key, group => group.ToList());
        }

        private void SetTaskText(string text)
        {
            if (_currentTaskText == null)
            {
                return;
            }

            _currentTaskText.text = text ?? string.Empty;
        }

        private sealed class RuntimeTaskStep
        {
            private readonly TaskStepDefinition _definition;

            public RuntimeTaskStep(TaskStepDefinition definition)
            {
                _definition = definition;
                ResetState();
            }

            public TaskStepType StepType => _definition.StepType;
            public bool IsSelfCompleted { get; private set; }
            public IReadOnlyList<TaskStepType> CompleteWhenAnyOfCompleted => _definition.CompleteWhenAnyOfCompleted;

            public void ResetState()
            {
                IsSelfCompleted = false;
            }

            public void SetCompleted(bool isCompleted)
            {
                IsSelfCompleted = isCompleted;
            }

            public string GetDisplayText()
            {
                return _definition.GetDisplayText();
            }
        }
    }
}
