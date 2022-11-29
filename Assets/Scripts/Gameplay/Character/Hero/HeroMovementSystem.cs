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
                .Filter<PlayerInputData>()
                .Inc<CharacterGrounded>()
                .End();

            var inputPool = world.GetPool<PlayerInputData>();
            var movementPool = world.GetPool<Movement>();
            var attackPool = world.GetPool<HeroAttack>();

            foreach (var e in entities)
            {
                ref var input = ref inputPool.Get(e);
                ref var movement = ref movementPool.Get(e);
                ref var attack = ref attackPool.Get(e);
                
                if ((input.IsMoved))
                    ChangeAcceleration(ref movement, (input.IsRunning) ? 1f : 0.6f, config.PlayerData.AccelerationTime);
                else
                    ChangeAcceleration(ref movement, 0f, config.PlayerData.AccelerationReleaseTime);
                           
                movement.CurrentSpeed = config.PlayerData.Speed * movement.Acceleration;

                var velocity = GetHorizontalMovementVelocity
                (
                    movement.Body.velocity, 
                    input.Direction, 
                    movement.CurrentSpeed
                );

                movement.Body.AddForce(velocity);
            }
        }             


        private void ChangeAcceleration(ref Movement movement, float targetAcceleration, float time)
        {
            movement.Acceleration = Mathf.Lerp
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