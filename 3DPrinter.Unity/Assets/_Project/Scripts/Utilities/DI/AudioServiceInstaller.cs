using _Project.Scripts.Audio;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Utilities.DI
{
    public class AudioServiceInstaller : MonoInstaller
    {
        [SerializeField] private AudioService _audioService;

        public override void InstallBindings()
        {
            if (_audioService == null)
            {
                Debug.LogError("[AudioServiceInstaller.InstallBindings] AudioService reference is missing.");
                return;
            }

            Container.Bind<AudioService>().FromInstance(_audioService).AsSingle();
            Container.QueueForInject(_audioService);
        }

        public override void Start()
        {
            _audioService?.Initialize();
        }
    }
}
