using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class HeroChangeHorizontalVelocitySystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var entities = world
                .Filter<Hero>()
                .Inc<CharacterCommand>()
                .Inc<CharacterControllerMovement>()
                .Inc<Translation>()
                .End();

            var heroPool = world.GetPool<Hero>();
            var commandPool = world.GetPool<CharacterCommand>();
            var movementPool = world.GetPool<CharacterControllerMovement>();
            var translationPool = world.GetPool<Translation>();
            var groundedPool = world.GetPool<CharacterGrounded>();
            var sitingPool = world.GetPool<CharacterSitDown>();
            var stunPool = world.GetPool<Stun>();

            foreach (var ent in entities)
            {
                ref var hero = ref heroPool.Get(ent);
                ref var command = ref commandPool.Get(ent);
                ref var movement = ref movementPool.Get(ent);
                ref var translation = ref translationPool.Get(ent);

                var isGrounded = groundedPool.Has(ent);
                var isSitting = sitingPool.Has(ent);

                if (isGrounded && isSitting)
                {
                    movement.Acceleration = 0f;
                    movement.HorizontalVelocity = Vector3.zero;
                    continue;
                }

                var speed = (!stunPool.Has(ent)) ? hero.Data.Speed : 0f;

                ApplyAcceleration(ref movement, ref command, ref hero);
                ApplySpeed(ref movement, speed, isGrounded);
                ChangeVelocity(ref movement, ref translation, ref command, ref hero, isGrounded);
            }
        }


        private void ChangeVelocity(ref CharacterControllerMovement movement, ref Translation translation, 
            ref CharacterCommand command, ref Hero hero, bool isGrounded)
        {
            var newVelocity = GetMovementVelocity
            (
                movement.CharacterController.velocity,
                translation.Value.TransformDirection(command.Direction),
                movement.CurrentSpeed
            );

            var changeTime = (isGrounded) ?
                hero.Data.ChangeVelocityTime :
                hero.Data.ChangeVelocityTime * hero.Data.ChangeVelocityTimeMultiplier;

            movement.HorizontalVelocity = Vector3.Lerp
            (
                movement.HorizontalVelocity,
                newVelocity,
                Time.deltaTime * changeTime
            );
        }


        private void ApplyAcceleration(ref CharacterControllerMovement movement, ref CharacterCommand command, ref Hero hero)
        {
            if (command.IsMoved)
            {
                var accelerationValue = (command.IsRunning) ?
                    hero.Data.AccelerationRun :
                    hero.Data.AccelerationWalk;

                ChangeAcceleration(ref movement, accelerationValue, hero.Data.AccelerationTime);
            }
            else
            {
                ChangeAcceleration(ref movement, 0f, hero.Data.AccelerationReleaseTime);
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