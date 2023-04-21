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
                .Inc<CharacterView>()
                .Inc<MovementAI>()
                .End();

            var deathPool = world.GetPool<Death>();
            var viewPool = world.GetPool<CharacterView>();
            var enemyPool = world.GetPool<Enemy>();
            var movementPool = world.GetPool<MovementAI>();

            foreach (var e in enemyEntities)
            {
                ref var death = ref deathPool.Get(e);
                ref var view = ref viewPool.Get(e);
                ref var enemy = ref enemyPool.Get(e);
                ref var movement = ref movementPool.Get(e);
                
                movement.NavAgent.speed = 0f;
                
                death.Timer -= Time.deltaTime;

                if (death.Timer <= 0f)
                {
                    enemy.PoolItem.ReturnToStorage();
                    CreateDeathEvent(world, ref view);
                    
                    world.DelEntity(e);
                }   
            }
        }
        

        private void CreateDeathEvent(EcsWorld world, ref CharacterView view)
        {
            var entity = world.NewEntity();
            ref var evt = ref world.GetPool<DeathVfxEvent>().Add(entity);
            evt.Position = view.HipBone.position;
        }
    }
}