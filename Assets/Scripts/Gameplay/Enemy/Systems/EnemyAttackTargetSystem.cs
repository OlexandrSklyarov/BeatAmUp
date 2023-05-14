using Leopotam.EcsLite;

namespace BT
{
    public sealed class EnemyAttackTargetSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var enemies = world
                .Filter<EnemyNavigation>()
                .Inc<AttackState>()
                .Inc<EnemyTarget>()
                .Inc<CharacterView>()
                .Inc<Translation>()
                .End();

            var navigationPool = world.GetPool<EnemyNavigation>();
            var attackStatePool = world.GetPool<AttackState>();
            var viewPool = world.GetPool<CharacterView>();
            var targetPool = world.GetPool<EnemyTarget>();
            var translationPool = world.GetPool<Translation>();

            foreach (var ent in enemies)
            {
                ref var navigation = ref navigationPool.Get(ent);
                ref var attackState = ref attackStatePool.Get(ent);  
                ref var view = ref viewPool.Get(ent);  
                ref var target = ref targetPool.Get(ent);  
                ref var tr = ref translationPool.Get(ent);  

                if (IsTargetFar(ref attackState, ref target, ref tr))
                {
                    navigation.Destination = target.MyTarget.position;
                    navigation.StopDistance = target.TargetRadius + view.BodyRadius + attackState.AttackDistance;
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