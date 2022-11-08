using Gameplay.Character.Hero;
using Leopotam.EcsLite;
using Services.Data;
using UnityEngine;

namespace Gameplay.Character.Hero
{
    public class HeroAnimationSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var config = systems.GetShared<SharedData>().Config;

            var world = systems.GetWorld();

            var entities = world
                .Filter<CharacterView>()
                .Inc<PlayerInputData>()
                .Inc<Movement>()
                .Inc<HeroTag>()
                .End();

            var viewPool = world.GetPool<CharacterView>();
            var inputPool = world.GetPool<PlayerInputData>();
            var movementPool = world.GetPool<Movement>();

            foreach(var e in entities)
            {
                ref var view = ref viewPool.Get(e);
                ref var input = ref inputPool.Get(e);
                ref var movement = ref movementPool.Get(e);

                var sqVelocity = movement.Body.velocity.sqrMagnitude;
                var isWalk = movement.IsGround && sqVelocity > 0f;
                var isFalling = !movement.IsGround && movement.Body.velocity.y < 0f;
                var isJumping = movement.IsGround && input.IsJump;

                var speedProgress = movement.CurrentSpeed / config.PlayerData.Speed;
                speedProgress = Mathf.Min(speedProgress, sqVelocity);

                view.Animator.SetBool(ConstPrm.Animation.MOVE, isWalk);
                view.Animator.SetFloat(ConstPrm.Animation.MOVE_SPEED, speedProgress);
                view.Animator.SetBool(ConstPrm.Animation.GROUND, movement.IsGround);
                view.Animator.SetBool(ConstPrm.Animation.FALLING, isFalling);

                if (isJumping) 
                {
                    view.Animator.SetTrigger(ConstPrm.Animation.JUMP);      
                }        
            }
        }
    }
}