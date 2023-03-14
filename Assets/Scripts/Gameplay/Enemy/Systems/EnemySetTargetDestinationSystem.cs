using Leopotam.EcsLite;
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

            var count = enemyEntities.GetEntitiesCount();
            var index = count;

            foreach (var e in enemyEntities)
            {
                ref var movement = ref movementPool.Get(e);
                ref var target = ref targetPool.Get(e);

                if (stunPool.Has(e))
                {
                    index--;
                    movement.NavAgent.isStopped = true;
                    continue;
                }

                var bodyRadius = movement.NavAgent.radius;
                var r = bodyRadius + ConstPrm.Enemy.TARGET_ENCIRCLEMENT_RADIUS;
                var destination = MathUtility.GetCirclePosition2D(target.MyTarget.position, 360, count, index, r);
                
                movement.NavAgent.SetDestination(destination);
                movement.NavAgent.stoppingDistance = bodyRadius * 2f;

                index--;

                UnityEngine.Debug.DrawLine(target.MyTarget.position, destination, UnityEngine.Color.red);
            }
        }        
    }
}