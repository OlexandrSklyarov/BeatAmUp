using Gameplay.Character.Hero;
using Leopotam.EcsLite;
using Services.Data;
using UnityEngine;

namespace Gameplay.Character.Hero
{
    public sealed class HeroMovementSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var config = systems.GetShared<SharedData>().Config;

            var entities = world.Filter<PlayerInputData>().End();
            var inputPool = world.GetPool<PlayerInputData>();
            var movementPool = world.GetPool<Movement>();
           
            foreach(var e in entities)
            {
                ref var input = ref inputPool.Get(e);
                ref var movement = ref movementPool.Get(e);

                if (movement.IsGround)
                {                    
                    movement.Acceleration = Mathf.Lerp
                    (
                        movement.Acceleration, 
                        (input.IsRunning) ? 1f : 0.5f, 
                        Time.deltaTime * config.PlayerData.Acceleration
                    );

                    movement.CurrentSpeed = config.PlayerData.Speed * movement.Acceleration;

                    movement.Body.AddForce(input.Direction * movement.CurrentSpeed);
                    movement.Body.drag = (input.IsMoved) ? config.PlayerData.MaxDrag : config.PlayerData.MinDrag;
                }
                else
                {
                    movement.Body.drag = config.PlayerData.MinDrag;
                }              
            }
        }
    }
}