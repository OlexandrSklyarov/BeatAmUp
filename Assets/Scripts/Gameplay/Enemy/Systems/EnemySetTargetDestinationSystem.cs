using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;
using Util;

namespace BT
{
    public sealed class EnemySetTargetDestinationSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var enemyEntities = world
                .Filter<Enemy>()
                .Inc<MovementAI>()
                .Inc<EnemyTarget>()
                .Exc<Death>()
                .End();

            var movementPool = world.GetPool<MovementAI>();
            var targetPool = world.GetPool<EnemyTarget>();
            var stunPool = world.GetPool<Stun>();

            var index = enemyEntities.GetEntitiesCount();
            var count = index;

            foreach (var e in enemyEntities)
            {                
                ref var movement = ref movementPool.Get(e);
                ref var target = ref targetPool.Get(e);

                if (stunPool.Has(e))
                {
                    index--;
                    movement.NavAgent.SetDestination(movement.MyTransform.position);
                    targetPool.Del(e);
                    continue;
                }
                
                var bodyRadius = movement.NavAgent.radius;
                var destination = GetaTargetAroundPosition(ref target, bodyRadius, count, index, 360f);

                movement.NavAgent.SetDestination(destination);
                movement.NavAgent.stoppingDistance = bodyRadius * 2f;
                
                index--;
            }
        }

        
        private Vector3 GetaTargetAroundPosition(ref EnemyTarget target, float bodyRadius, 
            int count, int index, float maxAngle)
        {
            var r = bodyRadius + ConstPrm.Enemy.TARGET_ENCIRCLEMENT_RADIUS;
            
            var destination = MathUtility.GetCirclePosition2D(
                target.MyTarget.position, maxAngle, count, index, r);
            
            UnityEngine.Debug.DrawLine(target.MyTarget.position, destination, UnityEngine.Color.red);
            return destination;
        }
    }
}