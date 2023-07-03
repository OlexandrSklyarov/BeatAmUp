using Leopotam.EcsLite;

namespace BT
{
    public sealed class TryResetHitEventSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var entities = world
                .Filter<TryHitEvent>()
                .Inc<Stun>()
                .End();

            var eventPool = world.GetPool<TryHitEvent>();

            foreach (var ent in entities)
            {
                eventPool.Del(ent);              
            }
        }
    }
}