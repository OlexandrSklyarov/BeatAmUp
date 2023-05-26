using System;
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
                .Filter<Enemy>()
                .Inc<Death>()
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

                ChangeDissolveValue(ref death, ref view);

                if (death.Timer <= 0f)
                {
                    enemy.PoolItem.ReturnToStorage();                    
                    world.DelEntity(e);
                }   
            }
        }


        private void ChangeDissolveValue(ref Death death, ref CharacterView view)
        {
            var progress = 1f - death.Timer / death.MaxTime;
            view.BodyMaterials.ChangeDissolveValue(progress);
        }       
    }
}