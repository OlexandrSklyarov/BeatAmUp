using Leopotam.EcsLite;

namespace BT
{
    public sealed class EnemyFindTargetHeroSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            
            var entities = world.Filter<Enemy>()
                .Inc<MovementAI>()
                .Exc<EnemyTarget>()
                .End();

            var heroes = world.Filter<HeroTag>()
                .Inc<CharacterControllerMovement>()
                .Exc<Death>()
                .End();

            var enemyTargetPool = world.GetPool<EnemyTarget>();
            var movementPool = world.GetPool<MovementAI>();
            var heroMovement = world.GetPool<CharacterControllerMovement>();

            foreach(var e in entities)
            {
                ref var aiComp = ref movementPool.Get(e);

                foreach(var h in heroes)
                {
                    if (enemyTargetPool.Has(e)) continue;

                    ref var heroTR = ref heroMovement.Get(h).Transform;

                    var sqDist = (aiComp.MyTransform.position - heroTR.position).sqrMagnitude;
                    var r = ConstPrm.Enemy.ViewTargetRadius * ConstPrm.Enemy.ViewTargetRadius;
                    
                    if (sqDist <= r)
                    {
                        ref var targetComp = ref enemyTargetPool.Add(e);
                        targetComp.MyTarget = heroTR;
                        Util.Debug.PrintColor($"I see target {heroTR.name}", UnityEngine.Color.yellow);
                    }
                }
            }
        } 
    }
}