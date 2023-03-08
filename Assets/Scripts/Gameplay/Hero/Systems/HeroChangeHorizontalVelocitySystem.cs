using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class HeroChangeHorizontalVelocitySystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var config = systems.GetShared<SharedData>().Config;

            var entities = world
                .Filter<CharacterCommand>()
                .Inc<CharacterControllerMovement>()
                .End();

            var commandPool = world.GetPool<CharacterCommand>();
            var movementPool = world.GetPool<CharacterControllerMovement>();
            var groundedPool = world.GetPool<CharacterGrounded>();
            var sitingPool = world.GetPool<CharacterSitDown>();

            foreach (var e in entities)
            {
                ref var command = ref commandPool.Get(e);
                ref var movement = ref movementPool.Get(e);

                var isGrounded = groundedPool.Has(e);
                var isSitting = sitingPool.Has(e);

                if (isGrounded && isSitting)
                {
                    movement.Acceleration = 0f;
                    movement.HorizontalVelocity = Vector3.zero;
                    continue;
                }

                ApplyAcceleration(ref movement, ref command, config);
                ApplySpeed(ref movement, config.PlayerData.Speed, isGrounded);
                ChangeVelocity(ref movement, config, isGrounded);
            }
        }


        private void ChangeVelocity(ref CharacterControllerMovement movement, GameConfig config, bool isGrounded)
        {
            var newVelocity = GetMovementVelocity
            (
                movement.characterController.velocity,
                movement.Direction,
                movement.CurrentSpeed
            );

            var changeTime = (isGrounded) ?
                config.PlayerData.ChangeVelocityTime :
                config.PlayerData.ChangeVelocityTime * config.PlayerData.ChangeVelocityTimeMultiplier;

            movement.HorizontalVelocity = Vector3.Lerp
            (
                movement.HorizontalVelocity,
                newVelocity,
                Time.deltaTime * changeTime
            );
        }


        private void ApplyAcceleration(ref CharacterControllerMovement movement, ref CharacterCommand command, GameConfig config)
        {
            if (command.IsMoved)
            {
                var accelerationValue = (command.IsRunning) ?
                    config.PlayerData.AccelerationRun :
                    config.PlayerData.AccelerationWalk;

                ChangeAcceleration(ref movement, accelerationValue, config.PlayerData.AccelerationTime);
            }
            else
            {
                ChangeAcceleration(ref movement, 0f, config.PlayerData.AccelerationReleaseTime);
            }
        }


        private void ApplySpeed(ref CharacterControllerMovement movement, float speed, bool isHasGrounded)
        {
            if (isHasGrounded)
            {
                movement.CurrentSpeed = speed * movement.Acceleration;
                movement.PreviousSpeed = movement.CurrentSpeed;
            }
            else
            {
                movement.CurrentSpeed = movement.PreviousSpeed;
            }
        }


        private void ChangeAcceleration(ref CharacterControllerMovement movement, float targetAcceleration, float time)
        {
            movement.Acceleration = Mathf.MoveTowards
            (
                movement.Acceleration,
                targetAcceleration,
                Time.deltaTime * time
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