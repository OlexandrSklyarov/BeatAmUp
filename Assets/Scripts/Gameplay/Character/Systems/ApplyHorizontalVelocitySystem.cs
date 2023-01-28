using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class ApplyHorizontalVelocitySystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var entities = world
                .Filter<Movement>()
                .End();
                
            var movementPool = world.GetPool<Movement>();

            foreach (var e in entities)
            {    
                ref var movement = ref movementPool.Get(e);  
                movement.characterController.Move(movement.HorizontalVelocity * Time.deltaTime);            
            }
        }
    }
}