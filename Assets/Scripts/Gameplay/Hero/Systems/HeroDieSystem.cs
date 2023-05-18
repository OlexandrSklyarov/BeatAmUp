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
                .Inc<CharacterView>()
                .End();

            var deathPool = world.GetPool<Death>();
            var viewPool = world.GetPool<CharacterView>();
            var movementCommandPool = world.GetPool<MovementCommand>();

            foreach (var e in entities)
            {
                ref var death = ref deathPool.Get(e);
                ref var view = ref viewPool.Get(e);
                
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