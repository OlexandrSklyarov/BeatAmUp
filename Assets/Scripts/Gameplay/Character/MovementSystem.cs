using Gameplay.Character.Hero;
using Leopotam.EcsLite;
using Services.Data;
using UnityEngine;

namespace Gameplay.Character
{
    public class MovementSystem : IEcsRunSystem
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
                    var speed = config.PlayerData.Speed;
                    movement.Body.AddForce(input.Direction * speed);
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