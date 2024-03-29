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
                .Filter<Hero>()
                .Inc<MovementCommand>()
                .Inc<CharacterControllerMovement>()
                .Inc<Translation>()
                .End();

            var heroPool = world.GetPool<Hero>();
            var commandPool = world.GetPool<MovementCommand>();
            var movementPool = world.GetPool<CharacterControllerMovement>();
            var translationPool = world.GetPool<Translation>();
            var groundedPool = world.GetPool<CharacterGrounded>();
            var stunPool = world.GetPool<Stun>();

            foreach (var ent in entities)
            {
                ref var hero = ref heroPool.Get(ent);
                ref var command = ref commandPool.Get(ent);
                ref var movement = ref movementPool.Get(ent);
                ref var translation = ref translationPool.Get(ent);

                var isGrounded = groundedPool.Has(ent);
                
                var data = config.Heroes[hero.ID].Data;
                var speed = (!stunPool.Has(ent)) ? data.Speed : 0f;

                ApplyAcceleration(ref movement, ref command, data);
                ApplySpeed(ref movement, speed, isGrounded);
                ChangeVelocity(ref movement, ref translation, ref command, isGrounded, data);
            }
        }


        private void ChangeVelocity(ref CharacterControllerMovement movement, ref Translation translation, 
            ref MovementCommand command, bool isGrounded, HeroData data)
        {
            var newVelocity = GetMovementVelocity
            (
                movement.CharacterController.velocity,
                translation.Value.TransformDirection(command.Direction),
                movement.CurrentSpeed
            );

            var changeTime = (isGrounded) ?
                data.ChangeVelocityTime :
                data.ChangeVelocityTime * data.ChangeVelocityTimeMultiplier;

            movement.HorizontalVelocity = Vector3.Lerp
            (
                movement.HorizontalVelocity,
                newVelocity,
                Time.deltaTime * changeTime
            );
        }


        private void ApplyAcceleration(ref CharacterControllerMovement movement, 
            ref MovementCommand command, HeroData data)
        {
            if (command.IsMoved)
            {
                var accelerationValue = (command.IsRunning) ?
                    data.AccelerationRun :
                    data.AccelerationWalk;

                ChangeAcceleration(ref movement, accelerationValue, data.AccelerationTime);
            }
            else
            {
                ChangeAcceleration(ref movement, 0f, data.AccelerationReleaseTime);
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
            movement.Acceleration = Mathf.Lerp
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