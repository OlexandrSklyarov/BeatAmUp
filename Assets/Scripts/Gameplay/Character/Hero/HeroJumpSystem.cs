using Gameplay.Character.Hero;
using Leopotam.EcsLite;
using Services.Data;
using UnityEngine;

namespace Gameplay.Character.Hero
{
    public class HeroJumpSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var entities = world.Filter<HeroTag>()
                .Inc<PlayerInputData>()
                .Inc<Movement>()
                .End();
                
            var inputPool = world.GetPool<PlayerInputData>();
            var movementPool = world.GetPool<Movement>();

            foreach (var e in entities)
            {
                ref var input = ref inputPool.Get(e);
                ref var movement = ref movementPool.Get(e);

                if (movement.IsGround && input.IsJump)
                {
                    var jumpForce = data.Config.PlayerData.JumpForce;
                    movement.Body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                }
            }
        }
    }
}