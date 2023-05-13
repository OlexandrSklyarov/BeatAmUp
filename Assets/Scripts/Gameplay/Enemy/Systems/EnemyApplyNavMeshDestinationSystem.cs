using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class EnemyApplyNavMeshDestinationSystem : IEcsRunSystem
    {       
        public void Run(IEcsSystems systems)
        {           
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var enemies = world
                .Filter<Enemy>()
                .Inc<MovementAI>()
                .Exc<Death>()
                .End();

            var movementPool = world.GetPool<MovementAI>();
            var stunPool = world.GetPool<Stun>();
            var blockMovementPool = world.GetPool<BlockMovement>();

            foreach (var ent in enemies)
            {
                ref var movement = ref movementPool.Get(ent);

                if (stunPool.Has(ent) || blockMovementPool.Has(ent))
                {
                    movement.NavAgent.speed = 0f;
                    movement.NavAgent.velocity = Vector3.zero;
                    movement.Destination = Vector3.zero;
                    continue;
                }

                var bodyRadius = movement.NavAgent.radius;

                movement.NavAgent.SetDestination(movement.Destination);
                movement.NavAgent.stoppingDistance = bodyRadius * 2f;
                movement.NavAgent.speed = GetRandomSpeed(data);
            }
        }


        private float GetRandomSpeed(SharedData data)
        {
            var max = data.Config.EnemyConfig.Movement.Speed;
            var min = max * 0.7f;

            return UnityEngine.Random.Range(min, max);
        }
    }
}