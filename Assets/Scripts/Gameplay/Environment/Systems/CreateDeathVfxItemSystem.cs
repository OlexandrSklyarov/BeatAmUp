using Leopotam.EcsLite;

namespace BT
{
    public sealed class CreateDeathVfxItemSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var damageReceivers = world
                .Filter<DeathVfxEvent>()
                .End();
            
            var eventPool = world.GetPool<DeathVfxEvent>();
            var vfxViewPool = world.GetPool<VfxView>();
            
            foreach (var ent in damageReceivers)
            {
                ref var evt = ref eventPool.Get(ent);

                GameplayExtensions.CreateVfxEntity(world, vfxViewPool, data, evt.Position, VfxType.CHARACTER_DEATH);
                
                eventPool.Del(ent);
            }
        }
    }
}