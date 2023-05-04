using System;
using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class EnemyBodyRotateSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var entities = world
                .Filter<Enemy>()
                .Inc<Translation>()
                .Inc<CharacterView>()
                .Inc<MovementAI>()
                .Exc<RagdollState>()
                .Exc<Stun>()
                .Exc<Death>()
                .End();

            var translationPool = world.GetPool<Translation>();
            var viewPool = world.GetPool<CharacterView>();
            var movementAIPool = world.GetPool<MovementAI>();
            var targetPool = world.GetPool<EnemyTarget>();

            foreach (var e in entities)
            {
                ref var translation = ref translationPool.Get(e);
                ref var view = ref viewPool.Get(e);
                ref var movement = ref movementAIPool.Get(e);  
                
                if (targetPool.Has(e))
                {
                    ref var target = ref targetPool.Get(e);
                    var dirToTarget = target.MyTarget.position - translation.Value.position;

                    if (dirToTarget.sqrMagnitude < ConstPrm.Enemy.TARGET_FOCUS_DIST)
                    {
                        RotateBody(ref view, dirToTarget, data);
                    }
                    else
                    {
                        RotateViewToAgentVelocity(ref movement, ref view, data);
                    }
                }
                else
                {
                    RotateViewToAgentVelocity(ref movement, ref view, data);
                }
            }
        }
        

        private void RotateViewToAgentVelocity(ref MovementAI movement, ref CharacterView view, SharedData data)
        {
            if (movement.NavAgent.velocity.magnitude <= ConstPrm.Enemy.MIN_VELOCITY_OFFSET) return;
            
            RotateBody(ref view, movement.NavAgent.velocity.normalized, data);
        }


        private void RotateBody(ref CharacterView view, Vector3 dir, SharedData data)
        {
            view.ViewTransform.rotation = Quaternion.RotateTowards
            (
                view.ViewTransform.rotation,
                Util.Vector3Math.DirToQuaternion(dir),
                Time.deltaTime * data.Config.EnemyConfig.Movement.AngularSpeed
            );
        }
    }
}