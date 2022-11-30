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

            var entities = world
                .Filter<CharacterCommand>()
                .Inc<Movement>()
                .Inc<CharacterGrounded>()
                .End();

            var commandPool = world.GetPool<CharacterCommand>();
            var movementPool = world.GetPool<Movement>();

            foreach (var e in entities)
            {
                ref var com = ref commandPool.Get(e);
                ref var movement = ref movementPool.Get(e);
                
                if ((com.IsMoved))
                    ChangeAcceleration(ref movement, (com.IsRunning) ? 1f : 0.6f, config.PlayerData.AccelerationTime);
                else
                    ChangeAcceleration(ref movement, 0f, config.PlayerData.AccelerationReleaseTime);
                           
                movement.CurrentSpeed = config.PlayerData.Speed * movement.Acceleration;

                var velocity = GetHorizontalMovementVelocity
                (
                    movement.Body.velocity, 
                    com.Direction, 
                    movement.CurrentSpeed
                );

                movement.Body.AddForce(velocity);
            }
        }             


        private void ChangeAcceleration(ref Movement movement, float targetAcceleration, float time)
        {
            movement.Acceleration = Mathf.MoveTowards
            (
                movement.Acceleration,
                targetAcceleration,
                Time.deltaTime * time
            );
        }


        private Vector3 GetHorizontalMovementVelocity(Vector3 curVelocity, Vector3 inputDir, float speed)
        {
            var targetVelocity = inputDir * speed;
            targetVelocity.y = 0f;

            curVelocity.y = 0f;

            return Vector3.ClampMagnitude(targetVelocity - curVelocity, speed);
        }
    }
}