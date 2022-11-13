using Leopotam.EcsLite;
using Services.Data;
using UnityEngine;

namespace Gameplay.Character.Hero
{
    public sealed class HeroAnimationSystem : IEcsRunSystem
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
            var heroAttackPool = world.GetPool<HeroAttack>();

            foreach(var e in entities)
            {
                ref var view = ref viewPool.Get(e);
                ref var input = ref inputPool.Get(e);
                ref var movement = ref movementPool.Get(e);
                ref var attack = ref heroAttackPool.Get(e);

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

                if (movement.IsGround && input.IsKick || input.IsPunch) 
                {
                    var attackTrigger = GetAttackTrigger(ref attack);                    
                    if (!string.IsNullOrEmpty(attackTrigger)) view.Animator.SetTrigger(attackTrigger);  
                }    
            }
        }


        private string GetAttackTrigger(ref HeroAttack attack)
        {        
            if (attack.CurrentPunchState != PunchState.NONE)
            {
                return attack.CurrentPunchState switch
                {
                    PunchState.PUNCH_1 => ConstPrm.Animation.PUNCH_1,
                    PunchState.PUNCH_2 => ConstPrm.Animation.PUNCH_2,
                    PunchState.PUNCH_3 => ConstPrm.Animation.PUNCH_3,
                    PunchState.PUNCH_4 => ConstPrm.Animation.PUNCH_4,
                    _=> null
                };
            }

            return attack.CurrentKickState switch
            {
                KickState.KICK_1 => ConstPrm.Animation.KICK_1,
                KickState.KICK_2 => ConstPrm.Animation.KICK_2,                
                _=> null
            };
        }
    }
}