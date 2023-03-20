using Leopotam.EcsLite;

namespace BT
{
    public sealed class InitWorldSystem : IEcsInitSystem
    {
        public void Init(IEcsSystems systems)
        {
            Util.Debug.Print("Init world...");

            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();           
        }        
    }
}