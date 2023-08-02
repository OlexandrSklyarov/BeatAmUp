using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class HeroDieSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var entities = world
                .Filter<Hero>()
                .Inc<Death>()
                .End();

            var deathPool = world.GetPool<Death>();
            var movementCommandPool = world.GetPool<MovementCommand>();

            foreach (var e in entities)
            {
                ref var death = ref deathPool.Get(e);
                
                if (movementCommandPool.Has(e))
                {
                    movementCommandPool.Del(e);
                }
                
                death.Timer -= Time.deltaTime;

                if (death.Timer <= 0f)
                {
                    world.DelEntity(e);

                    Util.Debug.PrintColor($"Hero die!!!", Color.red);
                }   
            }
        }
    }
}