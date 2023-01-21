using Leopotam.EcsLite;

namespace BT
{
    public sealed class HeroAnimationSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var config = systems.GetShared<SharedData>().Config;

            var world = systems.GetWorld();

            var entities = world
                .Filter<CharacterView>()
                .Inc<CharacterCommand>()
                .Inc<Movement>()
                .Inc<HeroTag>()
                .End();

            var viewPool = world.GetPool<CharacterView>();
            var inputPool = world.GetPool<CharacterCommand>();
            var movementPool = world.GetPool<Movement>();
            var heroAttackPool = world.GetPool<HeroAttack>();
            var groundedPool = world.GetPool<CharacterGrounded>();

            foreach(var e in entities)
            {
                ref var view = ref viewPool.Get(e);
                ref var input = ref inputPool.Get(e);
                ref var movement = ref movementPool.Get(e);
                ref var attack = ref heroAttackPool.Get(e);

                var isGrounded = groundedPool.Has(e);

                var sqVelocity = movement.HorizontalVelocity.sqrMagnitude;
                var isWalk = isGrounded && sqVelocity > 0f;
                var isFalling = !isGrounded && movement.VerticalVelocity < 0f;                
                var isJumping = isGrounded && movement.VerticalVelocity > 0f;

                var vel = movement.HorizontalVelocity;
                vel.y = 0f;
                var speedProgress = vel.magnitude / config.PlayerData.Speed;
                
                view.Animator.SetBool(ConstPrm.Animation.MOVE, isWalk);
                view.Animator.SetFloat(ConstPrm.Animation.MOVE_SPEED, speedProgress);
                view.Animator.SetBool(ConstPrm.Animation.GROUND, isGrounded);
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


        private string GetAttackTrigger(ref HeroAttack attack)
        {        
            if (attack.CurrentPunch != null) return attack.CurrentPunch.StateName;
            if (attack.CurrentKick != null)return attack.CurrentKick.StateName;
            
            return string.Empty;
        }
    }
}