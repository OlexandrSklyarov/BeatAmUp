using Leopotam.EcsLite;

namespace BT
{
    public sealed class HeroAnimationSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var entities = world
                .Filter<Hero>()
                .Inc<CharacterView>()
                .Inc<CharacterCommand>()
                .Inc<CharacterControllerMovement>()                
                .End();

            var heroPool = world.GetPool<Hero>();
            var viewPool = world.GetPool<CharacterView>();
            var inputPool = world.GetPool<CharacterCommand>();
            var movementPool = world.GetPool<CharacterControllerMovement>();
            var heroAttackPool = world.GetPool<CharacterAttack>();
            var groundedPool = world.GetPool<CharacterGrounded>();
            var sittingPool = world.GetPool<CharacterSitDown>();

            foreach(var e in entities)
            {
                ref var hero = ref heroPool.Get(e);
                ref var view = ref viewPool.Get(e);
                ref var input = ref inputPool.Get(e);
                ref var movement = ref movementPool.Get(e);
                ref var attack = ref heroAttackPool.Get(e);

                var isGrounded = groundedPool.Has(e);
                var isSitting = sittingPool.Has(e);
                var velMagnitude = movement.HorizontalVelocity.magnitude;
                var isFalling = !isGrounded && movement.VerticalVelocity < 0f;                
                var isJumping = !isGrounded && movement.VerticalVelocity > 0f;  
                var speedProgress = velMagnitude / hero.Data.Speed;
                
                view.Animator.SetFloat(ConstPrm.Animation.MOVE_SPEED, speedProgress);
                view.Animator.SetBool(ConstPrm.Animation.GROUND, isGrounded);
                view.Animator.SetBool(ConstPrm.Animation.SITTING, isSitting);
                view.Animator.SetBool(ConstPrm.Animation.FALLING, isFalling);
                view.Animator.SetFloat(ConstPrm.Animation.VERTICAL_VELOCITY, movement.VerticalVelocity);

                if (isJumping) 
                {
                    view.Animator.SetTrigger(ConstPrm.Animation.JUMP);      
                }   

                if (isGrounded && input.IsKick || input.IsPunch) 
                {
                    var attackTrigger = GetAttackTrigger(ref attack);                    
                    if (!string.IsNullOrEmpty(attackTrigger)) view.Animator.SetTrigger(attackTrigger);  
                }    
            }
        }


        private string GetAttackTrigger(ref CharacterAttack attack)
        {        
            if (attack.CurrentPunch != null) return attack.CurrentPunch.StateName;
            if (attack.CurrentKick != null) return attack.CurrentKick.StateName;
            
            return string.Empty;
        }
    }
}