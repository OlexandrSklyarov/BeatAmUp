using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class ResetHitCountSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var entities = world
                .Filter<HitCounter>()
                .Exc<Death>()
                .End();            

            var hitCounterPool = world.GetPool<HitCounter>();

            foreach (var e in entities)
            {
                ref var counter = ref hitCounterPool.Get(e);

                counter.HitResetTimer -= Time.deltaTime;

                if (counter.HitResetTimer <= 0f)
                {
                    hitCounterPool.Del(e);
                }
            }
        }
    }
}