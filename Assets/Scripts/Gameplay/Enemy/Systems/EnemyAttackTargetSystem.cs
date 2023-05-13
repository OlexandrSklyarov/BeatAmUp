using Leopotam.EcsLite;

namespace BT
{
    public sealed class EnemyAttackTargetSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var enemies = world
                .Filter<MovementAI>()
                .Inc<AttackState>()
                .Inc<EnemyTarget>()
                .Inc<Translation>()
                .End();

            var movementPool = world.GetPool<MovementAI>();
            var attackStatePool = world.GetPool<AttackState>();
            var targetPool = world.GetPool<EnemyTarget>();
            var translationPool = world.GetPool<Translation>();

            foreach (var ent in enemies)
            {
                ref var movement = ref movementPool.Get(ent);
                ref var attackState = ref attackStatePool.Get(ent);  
                ref var target = ref targetPool.Get(ent);  
                ref var tr = ref translationPool.Get(ent);  

                if (IsTargetFar(ref attackState, ref target, ref tr))
                {
                    movement.Destination = target.MyTarget.position;
                }
                else
                {
                    //attack
                    Util.Debug.PrintColor("Enemy Attack", UnityEngine.Color.red);
                }                
            }   
        }


        private bool IsTargetFar(ref AttackState attackState, ref EnemyTarget target, ref Translation tr)
        {
            var sqDist = (target.MyTarget.position - tr.Value.transform.position).sqrMagnitude;
            var minDist = attackState.AttackDistance * attackState.AttackDistance;
            return sqDist > minDist;
        }
    }
}