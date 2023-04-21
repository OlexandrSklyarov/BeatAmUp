using Leopotam.EcsLite;

namespace BT
{
    public sealed class CreateShakeCameraEventSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();
            
            var cameraEntities = world.Filter<GameCamera>() .End();
            var cameraPool = world.GetPool<GameCamera>();
            
            TryCreateDamageCameraShakeEvent(world, cameraEntities, cameraPool, data);
        }

        private void TryCreateDamageCameraShakeEvent(EcsWorld world, EcsFilter cameraEntities, 
            EcsPool<GameCamera> cameraPool, SharedData data)
        {
            var damageEntities = world.Filter<TakeDamageEvent>().End();
            var damageEventPool = world.GetPool<TakeDamageEvent>();

            foreach (var ent in damageEntities)
            {
                ref var damage = ref damageEventPool.Get(ent);

                foreach (var camEnt in cameraEntities)
                {

                    if (damage.IsPowerDamage || damage.IsHammeringDamage)
                    {
                        AddShakeCameraEvent(world, data, camEnt);
                    }
                }
            }
        }


        private void AddShakeCameraEvent(EcsWorld world, SharedData data, int cameraEntity)
        {
            var shakeEventPool = world.GetPool<ShakeCameraEvent>();

            if (shakeEventPool.Has(cameraEntity)) return;
            
            ref var evt = ref shakeEventPool.Add(cameraEntity);
            evt.Timer = data.Config.CameraConfig.ShakeDuration;
        }
    }
}