using Leopotam.EcsLite;
using UnityEngine;

namespace Gameplay.Character
{
    public class ApplyGravitySystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
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
                    movement.VerticalVelocity = -2f;
                }
                
                movement.VerticalVelocity += Physics.gravity.y * Time.fixedDeltaTime;                
                
                movement.characterController.Move(Vector3.up * movement.VerticalVelocity * Time.fixedDeltaTime);
                Util.Debug.Print($"movement.VerticalVelocity {movement.VerticalVelocity}");
            }
        }
    }
}