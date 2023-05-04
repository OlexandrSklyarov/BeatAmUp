using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class EnemyAnimationSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var entities = world
                .Filter<Enemy>()
                .Inc<CharacterView>()
                .Inc<MovementAI>()
                .End();

            var viewPool = world.GetPool<CharacterView>();
            var movementAIPool = world.GetPool<MovementAI>();
            var stunPool = world.GetPool<Stun>();
            var standAnimPool = world.GetPool<StandUpAnimationEvent>();

            foreach (var ent in entities)
            {
                ref var view = ref viewPool.Get(ent);
                ref var movement = ref movementAIPool.Get(ent);
                
                var isStun = stunPool.Has(ent);
                var curSpeed = GetCurrentSpeed(ref movement, data);
                var (side, forward) = CalculateDirectionParam(ref movement, ref view);
                
                view.Animator.SetBool(ConstPrm.Animation.STUN, isStun);
                view.Animator.SetFloat(ConstPrm.Animation.SIDE_PRM, side * curSpeed, 0.25f, Time.deltaTime);
                view.Animator.SetFloat(ConstPrm.Animation.FORWARD_PRM, forward * curSpeed,0.25f, Time.deltaTime);

                if (standAnimPool.Has(ent))
                {
                    ref var evt = ref standAnimPool.Get(ent);
                    view.Animator.SetTrigger(GetStandTrigger(ref evt));
                    standAnimPool.Del(ent);
                }
            }
        }

        
        private string GetStandTrigger(ref StandUpAnimationEvent evt)
        {
            return (evt.IsFaceDown) ? ConstPrm.Animation.STAND_UP_FACE_DOWN : ConstPrm.Animation.STAND_UP_FACE_UP;
        }


        private (float side, float forward) CalculateDirectionParam(ref MovementAI aiMovement, ref CharacterView view)
        {
            var velocity = aiMovement.NavAgent.velocity.normalized;
            var forward = Vector3.Dot(velocity, view.ViewTransform.forward);
            var side = Vector3.Dot(velocity, view.ViewTransform.right);
            
            return (side, forward);
        }


        private float GetCurrentSpeed(ref MovementAI movement, SharedData data)
        {
            var agent = movement.NavAgent;

            var normalizeSpeed = (agent.velocity.magnitude >= agent.stoppingDistance) ?
                agent.speed / agent.velocity.magnitude : 0f;

            return data.Config.EnemyConfig.Animation.ChangeSpeedCurve.Evaluate(normalizeSpeed);
        }
    }
}