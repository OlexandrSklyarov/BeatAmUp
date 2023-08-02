using Leopotam.EcsLite;

namespace BT
{
    public sealed class ResetHitEventSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var stunnedEntities = world
                .Filter<AttackEvent>()
                .Inc<Stun>()
                .End();

            var deathEntities = world
                .Filter<AttackEvent>()
                .Inc<Death>()
                .End();

            var eventPool = world.GetPool<AttackEvent>();

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