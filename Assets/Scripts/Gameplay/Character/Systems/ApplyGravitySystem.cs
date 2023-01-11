using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class ApplyGravitySystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var config = systems.GetShared<SharedData>().Config;
            var world = systems.GetWorld();

            var entities = world
                .Filter<Movement>()
                .End();

            var movementPool = world.GetPool<Movement>();
            var groundedPool = world.GetPool<CharacterGrounded>();

            foreach(var e in entities)
            {
                ref var movement = ref movementPool.Get(e);

                var isHasGrounded = groundedPool.Has(e);

                if (isHasGrounded  && movement.VerticalVelocity < 0f)
                {
                    movement.VerticalVelocity = -config.CharacterData.MinVerticalVelocity;
                }
                
                movement.VerticalVelocity += Physics.gravity.y * Time.fixedDeltaTime; 
                movement.VerticalVelocity = Mathf.Max(Physics.gravity.y, movement.VerticalVelocity);
                
                movement.characterController.Move(Vector3.up * movement.VerticalVelocity * Time.fixedDeltaTime);
            }
        }
    }
}