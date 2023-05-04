using Leopotam.EcsLite;
using UnityEngine;
using Util;

namespace BT
{
    public sealed class EnemySetAIDestinationSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var enemyEntities = world
                .Filter<Enemy>()
                .Inc<MovementAI>()
                .Inc<EnemyTarget>()
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
                    movement.NavAgent.speed = 0f;
                    movement.NavAgent.velocity = Vector3.zero;
                    targetPool.Del(e);
                    continue;
                }

                var bodyRadius = movement.NavAgent.radius;
                var destination = GetaTargetAroundPosition(ref target, bodyRadius, count, index, 360f);

                movement.NavAgent.SetDestination(destination);
                movement.NavAgent.stoppingDistance = bodyRadius * 2f;
                movement.NavAgent.speed = GetRandomSpeed(data);
                
                index--;
            }
        }
        

        private float GetRandomSpeed(SharedData data)
        {
            var max = data.Config.EnemyConfig.Movement.Speed;
            var min = max * 0.7f;

            return UnityEngine.Random.Range(min, max);
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