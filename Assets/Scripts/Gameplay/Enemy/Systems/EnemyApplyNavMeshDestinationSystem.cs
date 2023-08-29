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
                .Filter<MovementAI>()
                .Inc<EnemyNavigation>()
                .Inc<Translation>()
                .End();

            var movementPool = world.GetPool<MovementAI>();
            var navigationPool = world.GetPool<EnemyNavigation>();
            var translationPool = world.GetPool<Translation>();
            var blockMovementPool = world.GetPool<BlockMovement>();


            foreach (var ent in enemies)
            {
                ref var movement = ref movementPool.Get(ent);
                ref var navigation = ref navigationPool.Get(ent);                
                ref var tr = ref translationPool.Get(ent);                

                if (blockMovementPool.Has(ent) || IsArrivedAtDestination(ref tr, ref navigation))
                {
                    movement.NavAgent.speed = 0f;
                    movement.NavAgent.velocity = Vector3.zero;
                    continue;
                }

                movement.NavAgent.SetDestination(navigation.Destination);
                movement.NavAgent.stoppingDistance = navigation.StopDistance;
                movement.NavAgent.speed = GetRandomSpeed(data);
            }
        }


        private bool IsArrivedAtDestination(ref Translation tr, ref EnemyNavigation navigation)
        {
            return navigation.Destination == Vector3.zero ||
                (navigation.Destination - tr.Value.position).sqrMagnitude <= navigation.StopDistance * navigation.StopDistance;
        }


        private float GetRandomSpeed(SharedData data)
        {
            var max = data.Config.EnemyConfig.Movement.Speed;
            var min = max * 0.7f;

            return UnityEngine.Random.Range(min, max);
        }
    }
}