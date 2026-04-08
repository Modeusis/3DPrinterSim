using UnityEngine;
using Zenject;

namespace _Project.Scripts.UI.Tasks
{
    public class TaskManagerUnityEventBridge : MonoBehaviour
    {
        [SerializeField] private TaskStepType _stepType;
        [SerializeField] [TextArea] private string _message;

        private TaskManager _taskManager;

        [Inject]
        public void Construct(TaskManager taskManager)
        {
            _taskManager = taskManager;
        }

        public void CompleteConfiguredStep()
        {
            _taskManager?.CompleteStep(_stepType);
        }

        public void UncompleteConfiguredStep()
        {
            _taskManager?.UncompleteStep(_stepType);
        }

        public void ShowConfiguredStep()
        {
            _taskManager?.SetCurrentStep(_stepType);
        }

        public void CompleteStepByIndex(int stepType)
        {
            _taskManager?.CompleteStep((TaskStepType)stepType);
        }

        public void UncompleteStepByIndex(int stepType)
        {
            _taskManager?.UncompleteStep((TaskStepType)stepType);
        }

        public void ShowStepByIndex(int stepType)
        {
            _taskManager?.SetCurrentStep((TaskStepType)stepType);
        }

        public void ShowConfiguredMessage()
        {
            _taskManager?.ShowMessage(_message);
        }

        public void ShowMessage(string message)
        {
            _taskManager?.ShowMessage(message);
        }

        public void ClearMessage()
        {
            _taskManager?.ClearMessage();
        }
    }
}
