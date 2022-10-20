using Gameplay.Character;
using Gameplay.Character.Hero;
using Leopotam.EcsLite;
using Services.Data;

namespace Gameplay.GameCamera
{
    public class InitCameraSystem : IEcsInitSystem
    {
        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>().WorldData;

            var entities = world.Filter<HeroTag>().End();
            var movementPool = world.GetPool<Movement>();

            foreach(var e in entities)
            {
                var target = movementPool.Get(e).Transform;

                data.GameVC.Follow = target;
                data.GameVC.LookAt = target;
            }
        }
    }
}