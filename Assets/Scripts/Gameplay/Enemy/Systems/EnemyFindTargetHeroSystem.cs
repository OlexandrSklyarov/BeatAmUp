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
                .Exc<Stun>()
                .End();

            var heroes = world.Filter<Hero>()
                .Inc<CharacterView>()
                .Exc<Death>()
                .End();

            var enemyTargetPool = world.GetPool<EnemyTarget>();
            var movementPool = world.GetPool<MovementAI>();
            var heroCharacterView = world.GetPool<CharacterView>();

            foreach(var e in entities)
            {
                ref var aiComp = ref movementPool.Get(e);

                foreach(var h in heroes)
                {
                    if (enemyTargetPool.Has(e)) continue;

                    ref var heroTR = ref heroCharacterView.Get(h).ViewTransform;

                    var sqDist = (aiComp.MyTransform.position - heroTR.position).sqrMagnitude;
                    var r = ConstPrm.Enemy.VIEW_TARGET_RADIUS * ConstPrm.Enemy.VIEW_TARGET_RADIUS;
                    
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