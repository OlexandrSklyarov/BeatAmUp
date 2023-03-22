using System;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.AI;

namespace BT
{
    public sealed class CharacterReleaseStunSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var entities = world
                .Filter<Stun>()
                .Exc<Death>()
                .End();

            var stunPool = world.GetPool<Stun>();
            var aiPool = world.GetPool<MovementAI>();
            var heroMovementPool = world.GetPool<CharacterControllerMovement>();

            foreach (var e in entities)
            {
                ref var stun = ref stunPool.Get(e);
                
                stun.Timer -= Time.deltaTime;

                var isStunEnd = stun.Timer <= 0f;

                if (isStunEnd) 
                {
                    stunPool.Del(e); 

                    if (aiPool.Has(e)) 
                        SetPositionNavMesh(world, aiPool.Get(e).NavAgent, e);
                    else if (heroMovementPool.Has(e))
                        SetPositionHero(world, heroMovementPool.Get(e).Transform, e);
                }               
            }
        }


        private void SetPositionHero(EcsWorld world, Transform transform, int entity)
        {
            
        }


        private void SetPositionNavMesh(EcsWorld world, NavMeshAgent navAgent, int entity)
        {
            var enemyPool = world.GetPool<Enemy>();

            if (!enemyPool.Has(entity)) return;

            ref var enemy = ref enemyPool.Get(entity);
            navAgent.Warp(enemy.ViewProvider.BodyHipsPosition);
        }
    }
}