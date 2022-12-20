using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class HeroJumpSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var entities = world.Filter<HeroTag>()
                .Inc<CharacterCommand>()
                .Inc<Movement>()
                .Inc<CharacterGrounded>()
                .End();
                
            var inputPool = world.GetPool<CharacterCommand>();
            var movementPool = world.GetPool<Movement>();

            foreach (var e in entities)
            {
                ref var input = ref inputPool.Get(e);
                ref var movement = ref movementPool.Get(e);

                if (input.IsJump)
                {
                    var jumpForce = data.Config.PlayerData.JumpForce;
                    movement.VerticalVelocity = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
                }
            }
        }
    }
}