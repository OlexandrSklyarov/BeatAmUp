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

            foreach (var e in entities)
            {
                ref var view = ref viewPool.Get(e);
                ref var movement = ref movementAIPool.Get(e);

                var isStun = stunPool.Has(e);
                var curSpeed = GetCurrentSpeed(ref movement, data);
                var (side, forward) = CalculateDirectionParam(ref movement, ref view);
                
                view.Animator.SetBool(ConstPrm.Animation.STUN, isStun);
                //view.Animator.SetFloat(ConstPrm.Animation.MOVE_SPEED, curSpeed, 0.3f, Time.deltaTime);
                view.Animator.SetFloat(ConstPrm.Animation.SIDE_PRM, side * curSpeed, 0.25f, Time.deltaTime);
                view.Animator.SetFloat(ConstPrm.Animation.FORWARD_PRM, forward * curSpeed,0.25f, Time.deltaTime);
            }
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