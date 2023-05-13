using Leopotam.EcsLite;

namespace BT
{
    public sealed class EnemyResetAttackStateSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var enemies = world
                .Filter<Enemy>()
                .Inc<Translation>()
                .Inc<EnemyTarget>()
                .Inc<AttackState>()
                .End();  

            var translationPool = world.GetPool<Translation>();
            var targetPool = world.GetPool<EnemyTarget>();
            var attackStatePool = world.GetPool<AttackState>();
            var ragdollStatePool = world.GetPool<RagdollState>();
            
            foreach(var ent in enemies)
            {
                ref var tr = ref translationPool.Get(ent);
                ref var target = ref targetPool.Get(ent);

                if (IsTargetClose(ref target, ref tr) || ragdollStatePool.Has(ent)) 
                    continue;               

                attackStatePool.Del(ent);
            }
        }
        

        private bool IsTargetClose(ref EnemyTarget target, ref Translation tr)
        {
            var sqDist = (target.MyTarget.position - tr.Value.position).sqrMagnitude;
            var minDist = ConstPrm.Enemy.TARGET_ENCIRCLEMENT_RADIUS * ConstPrm.Enemy.TARGET_ENCIRCLEMENT_RADIUS;
            return sqDist <= minDist;
        }
    }
}