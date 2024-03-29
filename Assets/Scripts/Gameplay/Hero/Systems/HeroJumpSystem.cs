using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class HeroJumpSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var config = systems.GetShared<SharedData>().Config;

            var entities = world.Filter<Hero>()
                .Inc<MovementCommand>()
                .Inc<CharacterControllerMovement>()
                .Inc<CharacterGrounded>()
                .End();
                
            var heroPool = world.GetPool<Hero>();
            var commandPool = world.GetPool<MovementCommand>();
            var movementPool = world.GetPool<CharacterControllerMovement>();

            foreach (var e in entities)
            {
                
                ref var hero = ref heroPool.Get(e);
                ref var command = ref commandPool.Get(e);
                ref var movement = ref movementPool.Get(e);
                
                if (!command.IsJump) 
                {
                    movement.IsJumpProcess = false;
                }

                if (!movement.IsJumpProcess && command.IsJump)
                {
                    var data = config.Heroes[hero.ID].Data;
                    var jumpForce = data.JumpForce;
                    movement.VerticalVelocity = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
                    movement.IsJumpProcess = true;
                }
            }
        }
    }
}