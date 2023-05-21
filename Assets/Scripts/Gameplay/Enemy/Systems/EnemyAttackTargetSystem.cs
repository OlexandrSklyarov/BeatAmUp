using Leopotam.EcsLite;
using Random = UnityEngine.Random;

namespace BT
{
    public sealed class EnemyAttackTargetSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var enemies = world
                .Filter<EnemyNavigation>()
                .Inc<AttackState>()
                .Inc<EnemyTarget>()
                .Inc<CharacterView>()
                .Inc<Translation>()
                .Inc<CombatCommand>()
                .Exc<BlockMovement>()
                .End();

            var navigationPool = world.GetPool<EnemyNavigation>();
            var attackStatePool = world.GetPool<AttackState>();
            var combatCommandPool = world.GetPool<CombatCommand>();
            var viewPool = world.GetPool<CharacterView>();
            var targetPool = world.GetPool<EnemyTarget>();
            var translationPool = world.GetPool<Translation>();
            var blockPool = world.GetPool<BlockMovement>();

            foreach (var ent in enemies)
            {
                ref var navigation = ref navigationPool.Get(ent);
                ref var attackState = ref attackStatePool.Get(ent);
                ref var view = ref viewPool.Get(ent);
                ref var target = ref targetPool.Get(ent);
                ref var tr = ref translationPool.Get(ent);
                ref var combat = ref combatCommandPool.Get(ent);

                var stopDistance = target.TargetRadius + view.BodyRadius + attackState.AttackDistance;                

                if (IsTargetFar(ref target, ref tr, stopDistance))
                {
                    navigation.Destination = target.MyTarget.position;
                    navigation.StopDistance = stopDistance;
                }
                else
                {
                    //attack
                    Util.Debug.PrintColor("Enemy Attack", UnityEngine.Color.red);
                    ref var block = ref blockPool.Add(ent);
                    block.Timer = GetAttackDelay(data);

                    SetRandomCombatAction(ref combat);
                }
            }
        }


        private float GetAttackDelay(SharedData data)
        {
            return data.Config.EnemyConfig.Animation.AttackAnimationDelay + 
                Random.Range(1, 2);
        }


        private void SetRandomCombatAction(ref CombatCommand combat)
        {
            if (Random.Range(0, 100) > 50)
            {
                combat.IsKick = true;
            }
            else
            {
                combat.IsPunch = true;
            }
        }


        private bool IsTargetFar(ref EnemyTarget target,
            ref Translation tr, float stopDistance)
        {
            var sqDist = (target.MyTarget.position - tr.Value.transform.position).sqrMagnitude;
            var minDist = stopDistance * stopDistance;
            return sqDist > minDist;
        }
    }
}