using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class EnemyCharacterDieSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var enemyEntities = world
                .Filter<Death>()
                .Inc<Enemy>()
                .Inc<MovementAI>()
                .Inc<CharacterView>()
                .End();

            var deathPool = world.GetPool<Death>();
            var enemyPool = world.GetPool<Enemy>();
            var movementPool = world.GetPool<MovementAI>();
            var viewPool = world.GetPool<CharacterView>();

            foreach (var e in enemyEntities)
            {
                ref var death = ref deathPool.Get(e);
                ref var enemy = ref enemyPool.Get(e);
                ref var movement = ref movementPool.Get(e);
                ref var view = ref viewPool.Get(e);
                
                movement.NavAgent.isStopped = true;
                
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