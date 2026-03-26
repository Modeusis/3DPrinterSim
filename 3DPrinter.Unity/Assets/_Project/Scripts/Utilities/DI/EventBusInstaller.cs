using Game.Scripts.Utilities.Events;
using Zenject;

namespace _Project.Scripts.Utilities.DI
{
    public class EventBusInstaller : MonoInstaller
    {
        private EventBus _eventBus;
        
        public override void InstallBindings()
        {
            _eventBus = new EventBus();
            
            Container.Bind<EventBus>().FromInstance(_eventBus).AsSingle();
        }
    }
}