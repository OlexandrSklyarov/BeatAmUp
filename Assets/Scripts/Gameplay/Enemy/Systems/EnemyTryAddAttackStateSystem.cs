using Leopotam.EcsLite;

namespace BT
{
    public class EnemyTryAddAttackStateSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var enemies = world
                .Filter<Enemy>()
                .Inc<Translation>()
                .Inc<EnemyTarget>()
                .Exc<AttackState>()
                .Exc<Death>()
                .Exc<RagdollState>()
                .End();           

            var attackedEnemies = world
                .Filter<Enemy>()
                .Inc<AttackState>()
                .End();

            var translationPool = world.GetPool<Translation>();
            var targetPool = world.GetPool<EnemyTarget>();
            var attackStatePool = world.GetPool<AttackState>();

            if (attackedEnemies.GetEntitiesCount() >= ConstPrm.Enemy.MAX_ATTACKING_ENEMY_COUNT) return;

            foreach(var ent in enemies)
            {
                ref var target = ref targetPool.Get(ent);
                ref var tr = ref translationPool.Get(ent);

                if (!IsTargetClose(ref target, ref tr)) continue;
                if (!IsAttackСhance()) continue;

                attackStatePool.Add(ent);
            }
        }


        private bool IsAttackСhance()
        {
            return UnityEngine.Random.Range(0f, 1f) > 0.7f;
        }


        private bool IsTargetClose(ref EnemyTarget target, ref Translation tr)
        {
            var sqDist = (target.MyTarget.position - tr.Value.position).sqrMagnitude;
            var minDist = ConstPrm.Enemy.TARGET_ENCIRCLEMENT_RADIUS * ConstPrm.Enemy.TARGET_ENCIRCLEMENT_RADIUS;
            return sqDist <= minDist;
        }
    }
}