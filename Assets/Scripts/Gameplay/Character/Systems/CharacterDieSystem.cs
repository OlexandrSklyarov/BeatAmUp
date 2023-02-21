using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class CharacterDieSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var enemyEntities = world
                .Filter<Death>()
                .Inc<Enemy>()
                .End();

            var deathPool = world.GetPool<Death>();
            var enemyPool = world.GetPool<Enemy>();

            foreach (var e in enemyEntities)
            {
                ref var death = ref deathPool.Get(e);
                ref var enemy = ref enemyPool.Get(e);
                
                death.Timer -= Time.deltaTime;

                if (death.Timer <= 0f)
                {
                    enemy.ViewProvider.ReturnToStorage();
                    world.DelEntity(e);
                }   
            }
        }        
    }
}