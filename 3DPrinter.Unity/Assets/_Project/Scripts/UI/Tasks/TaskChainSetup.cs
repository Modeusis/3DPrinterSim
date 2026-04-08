using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.UI.Tasks
{
    [CreateAssetMenu(fileName = "TaskChainSetup", menuName = "Setup/Tasks/Task Chain")]
    public class TaskChainSetup : ScriptableObject
    {
        [SerializeField] private List<TaskStepDefinition> _steps = new List<TaskStepDefinition>();
        [SerializeField] [TextArea] private string _allTasksCompletedMessage = "All tasks completed.";

        public IReadOnlyList<TaskStepDefinition> Steps => _steps;
        public string AllTasksCompletedMessage => _allTasksCompletedMessage;
    }

    [Serializable]
    public class TaskStepDefinition
    {
        [field: SerializeField] public TaskStepType StepType { get; private set; }
        [field: SerializeField] [field: TextArea] public string TaskText { get; private set; }
        [SerializeField] private List<TaskStepType> _completeWhenAnyOfCompleted = new List<TaskStepType>();

        public IReadOnlyList<TaskStepType> CompleteWhenAnyOfCompleted => _completeWhenAnyOfCompleted;

        public string GetDisplayText()
        {
            return string.IsNullOrWhiteSpace(TaskText) ? StepType.ToString() : TaskText;
        }
    }
}
