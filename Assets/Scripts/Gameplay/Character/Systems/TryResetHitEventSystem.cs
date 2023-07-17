using Leopotam.EcsLite;

namespace BT
{
    public sealed class TryResetHitEventSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var stunnedEntities = world
                .Filter<TryHitEvent>()
                .Inc<Stun>()
                .End();

            var deathEntities = world
                .Filter<TryHitEvent>()
                .Inc<Death>()
                .End();

            var eventPool = world.GetPool<TryHitEvent>();

            foreach (var ent in stunnedEntities)
            {
                eventPool.Del(ent);              
            }

            foreach (var ent in deathEntities)
            {
                eventPool.Del(ent);              
            }
        }
    }
}