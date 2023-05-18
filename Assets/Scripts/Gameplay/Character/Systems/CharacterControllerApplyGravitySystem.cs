using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class CharacterControllerApplyGravitySystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var config = systems.GetShared<SharedData>().Config;
            var world = systems.GetWorld();

            var entities = world
                .Filter<CharacterControllerMovement>()
                .Inc<MovementCommand>()
                .End();

            var movementPool = world.GetPool<CharacterControllerMovement>();
            var commandPool = world.GetPool<MovementCommand>();
            var groundedPool = world.GetPool<CharacterGrounded>();

            foreach(var e in entities)
            {
                ref var movement = ref movementPool.Get(e);
                ref var command = ref commandPool.Get(e);
                var isHasGrounded = groundedPool.Has(e);

                ClampVerticalVelocity(ref movement, isHasGrounded, config);
                
                var gravityMultiplier = (!isHasGrounded && movement.IsJumpProcess && !command.IsJump) ? 
                    config.CharacterData.FallGravityMultiplier : 1f;

                movement.VerticalVelocity += Physics.gravity.y * gravityMultiplier * Time.deltaTime; 
                movement.VerticalVelocity = Mathf.Max(Physics.gravity.y, movement.VerticalVelocity);
                
                movement.CharacterController.Move(Vector3.up * movement.VerticalVelocity * Time.deltaTime);
            }
        }

        
        private void ClampVerticalVelocity(ref CharacterControllerMovement movement, bool isHasGrounded, GameConfig config)
        {
            if (!isHasGrounded) return;
            if (movement.VerticalVelocity >= 0f) return;
            
            movement.VerticalVelocity = -config.CharacterData.MinVerticalVelocity;
        }
    }
}