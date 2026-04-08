using _Project.Scripts.UI.Tasks;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Utilities.DI
{
    public class TaskManagerInstaller : MonoInstaller
    {
        [Header("Task Data")]
        [SerializeField] private TaskChainSetup _taskChainSetup;

        [Header("Task UI")]
        [SerializeField] private TMP_Text _currentTaskText;

        [Header("Message UI")]
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private CanvasGroup _messageCanvasGroup;

        [Space]
        [SerializeField] private float _messageFadeInDuration = 0.2f;
        [SerializeField] private float _messageFadeOutDuration = 0.2f;
        [SerializeField] private bool _autoHideMessages = true;
        [SerializeField] private float _messageAutoHideDelay = 1.5f;

        public override void InstallBindings()
        {
            if (_taskChainSetup == null)
            {
                Debug.LogError("[TaskManagerInstaller.InstallBindings] TaskChainSetup reference is missing.");
                return;
            }

            var taskManager = new TaskManager(
                _taskChainSetup,
                _currentTaskText,
                _messageText,
                _messageCanvasGroup,
                _messageFadeInDuration,
                _messageFadeOutDuration,
                _messageAutoHideDelay,
                _autoHideMessages);

            Container.Bind<TaskManager>().FromInstance(taskManager).AsSingle();
        }
    }
}
