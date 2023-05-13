using Leopotam.EcsLite;
using UnityEngine;
using Util;

namespace BT
{
    public sealed class EnemyAssigningPointNearTargetSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var enemyEntities = world
                .Filter<Enemy>()
                .Inc<MovementAI>()
                .Inc<EnemyTarget>()
                .Exc<AttackState>()
                .Exc<Death>()
                .Exc<RagdollState>()
                .End();

            var movementPool = world.GetPool<MovementAI>();
            var targetPool = world.GetPool<EnemyTarget>();
            var stunPool = world.GetPool<Stun>();
            var blockMovementPool = world.GetPool<BlockMovement>();

            var index = enemyEntities.GetEntitiesCount();
            var count = index;

            foreach (var e in enemyEntities)
            {                
                ref var movement = ref movementPool.Get(e);
                ref var target = ref targetPool.Get(e);

                if (stunPool.Has(e) || blockMovementPool.Has(e))
                {
                    index--;                    
                    targetPool.Del(e);
                    continue;
                }              
                
                movement.Destination = GetTargetAroundPosition(ref target, count, index, 360f);
                
                index--;
            }
        }     


        private Vector3 GetTargetAroundPosition(ref EnemyTarget target, int count, int index, float maxAngle)
        {        
            var destination = MathUtility.GetCirclePosition2D(
                target.MyTarget.position, maxAngle, count, index, target.TargetRadius);
            
            UnityEngine.Debug.DrawLine(target.MyTarget.position, destination, UnityEngine.Color.red);
            return destination;
        }
    }
}