using Leopotam.EcsLite;

namespace BT
{
    public sealed class CreateDamageVfxItemSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var damageReceivers = world
                .Filter<TakeDamageEvent>()
                .End();
            
            var damageEventPool = world.GetPool<TakeDamageEvent>();
            var vfxViewPool = world.GetPool<VfxView>();
            
            foreach (var e in damageReceivers)
            {
                ref var damageEvent = ref damageEventPool.Get(e);   
                GameplayExtensions.CreateVfxEntity(world, vfxViewPool, data, damageEvent.HitPoint, VfxType.CHARACTER_HIT); 
            }
        }
    }
}