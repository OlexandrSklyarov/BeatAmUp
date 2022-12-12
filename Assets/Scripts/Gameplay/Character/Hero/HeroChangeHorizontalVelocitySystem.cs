using System;
using Leopotam.EcsLite;
using Services.Data;
using UnityEngine;

namespace Gameplay.Character.Hero
{
    public sealed class HeroChangeHorizontalVelocitySystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var config = systems.GetShared<SharedData>().Config;

            var entities = world
                .Filter<CharacterCommand>()
                .Inc<Movement>()     
                .Inc<CharacterGrounded>()           
                .End();

            var commandPool = world.GetPool<CharacterCommand>();
            var movementPool = world.GetPool<Movement>();

            foreach (var e in entities)
            {
                ref var command = ref commandPool.Get(e);
                ref var movement = ref movementPool.Get(e);
                
                if ((command.IsMoved))
                    ChangeAcceleration(ref movement, (command.IsRunning) ? 1f : 0.6f, config.PlayerData.AccelerationTime);
                else
                    ChangeAcceleration(ref movement, 0f, config.PlayerData.AccelerationReleaseTime);
                           
                movement.CurrentSpeed = config.PlayerData.Speed * movement.Acceleration;
                
                var newVelocity = GetMovementVelocity
                ( 
                    movement.characterController.velocity,
                    command.Direction, 
                    movement.CurrentSpeed
                );                

                movement.HorizontalVelocity = Vector3.Lerp
                (
                    movement.HorizontalVelocity,
                    newVelocity,
                    Time.fixedDeltaTime * config.PlayerData.ChangeVelocityTime
                );
            }
        }
       

        private void ChangeAcceleration(ref Movement movement, float targetAcceleration, float time)
        {
            movement.Acceleration = Mathf.MoveTowards
            (
                movement.Acceleration,
                targetAcceleration,
                Time.fixedDeltaTime * time
            );
        }


        private Vector3 GetMovementVelocity(Vector3 curVelocity, Vector3 inputDir, float speed)
        {
            var targetVelocity = inputDir * speed;
            curVelocity.y = targetVelocity.y = 0f;
            return Vector3.ClampMagnitude(targetVelocity - curVelocity, speed);     
        }
    }
}