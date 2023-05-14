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
                .Filter<EnemyNavigation>()
                .Inc<CharacterView>()
                .Inc<EnemyTarget>()
                .End();

            var navigationPool = world.GetPool<EnemyNavigation>();
            var viewPool = world.GetPool<CharacterView>();
            var targetPool = world.GetPool<EnemyTarget>();
            var blockMovementPool = world.GetPool<BlockMovement>();

            var index = enemyEntities.GetEntitiesCount();
            var count = index;

            foreach (var e in enemyEntities)
            {                
                ref var navigation = ref navigationPool.Get(e);
                ref var target = ref targetPool.Get(e);
                ref var view = ref viewPool.Get(e);

                if (blockMovementPool.Has(e))
                {
                    index--;                    
                    targetPool.Del(e);
                    continue;
                }              
                
                navigation.Destination = GetTargetAroundPosition(ref target, count, index, 360f);
                navigation.StopDistance = view.BodyRadius;
                
                index--;
            }
        }     


        private Vector3 GetTargetAroundPosition(ref EnemyTarget target, int count, int index, float maxAngle)
        {        
            var destination = MathUtility.GetCirclePosition2D(
                target.MyTarget.position, maxAngle, count, index, target.MinVisualDistance);
            
            UnityEngine.Debug.DrawLine(target.MyTarget.position, destination, UnityEngine.Color.red);
            return destination;
        }
    }
}