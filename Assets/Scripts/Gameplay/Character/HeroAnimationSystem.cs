using Gameplay.Character.Hero;
using Leopotam.EcsLite;
using Services.Data;
using UnityEngine;

namespace Gameplay.Character
{
    public class HeroAnimationSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var config = systems.GetShared<SharedData>().Config;

            var entities = systems.GetWorld()
                .Filter<CharacterView>()
                .Inc<PlayerInputData>()
                .End();

            var viewPool = systems.GetWorld().GetPool<CharacterView>();
            var inputPool = systems.GetWorld().GetPool<PlayerInputData>();
            var movementPool = systems.GetWorld().GetPool<Movement>();

            foreach(var e in entities)
            {
                ref var view = ref viewPool.Get(e);
                ref var input = ref inputPool.Get(e);
                ref var movement = ref movementPool.Get(e);

                var sqVelocity = movement.Body.velocity.sqrMagnitude;
                var isWalk = movement.IsGround && sqVelocity > 0f;

                var speedProgress = movement.CurrentSpeed / config.PlayerData.Speed;
                speedProgress = Mathf.Min(speedProgress, sqVelocity);

                view.Animator.SetBool(ConstPrm.Animation.MOVE, isWalk);
                view.Animator.SetFloat(ConstPrm.Animation.MOVE_SPEED, speedProgress);
            }
        }
    }
}