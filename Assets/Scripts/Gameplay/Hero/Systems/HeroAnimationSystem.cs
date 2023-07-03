using Leopotam.EcsLite;

namespace BT
{
    public sealed class HeroAnimationSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var config = systems.GetShared<SharedData>().Config;

            var entities = world
                .Filter<Hero>()
                .Inc<CharacterView>()
                .Inc<CharacterControllerMovement>()                
                .End();

            var heroPool = world.GetPool<Hero>();
            var viewPool = world.GetPool<CharacterView>();
            var movementPool = world.GetPool<CharacterControllerMovement>();
            var groundedPool = world.GetPool<CharacterGrounded>();

            foreach(var e in entities)
            {
                ref var hero = ref heroPool.Get(e);
                ref var view = ref viewPool.Get(e);
                ref var movement = ref movementPool.Get(e);

                var data = config.Heroes[hero.ID].Data;

                var isGrounded = groundedPool.Has(e);
                var velMagnitude = movement.HorizontalVelocity.magnitude;
                var isFalling = !isGrounded && movement.VerticalVelocity < 0f;                
                var isJumping = !isGrounded && movement.VerticalVelocity > 0f;  
                var speedProgress = velMagnitude / data.Speed;
                
                view.Animator.SetFloat(ConstPrm.Animation.MOVE_SPEED, speedProgress);
                view.Animator.SetBool(ConstPrm.Animation.GROUND, isGrounded);
                view.Animator.SetBool(ConstPrm.Animation.FALLING, isFalling);
                view.Animator.SetFloat(ConstPrm.Animation.VERTICAL_VELOCITY, movement.VerticalVelocity);

                if (isJumping) view.Animator.SetTrigger(ConstPrm.Animation.JUMP);  
            }
        }
    }
}