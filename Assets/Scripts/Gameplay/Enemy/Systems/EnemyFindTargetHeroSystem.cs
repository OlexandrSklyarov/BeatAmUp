using Leopotam.EcsLite;

namespace BT
{
    public sealed class EnemyFindTargetHeroSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            
            var enemies = world.Filter<Enemy>()
                .Inc<Translation>()
                .Exc<EnemyTarget>()
                .End();

            var heroes = world.Filter<Hero>()
                .Inc<CharacterView>()
                .Exc<Death>()
                .End();

            var enemyTargetPool = world.GetPool<EnemyTarget>();
            var translationPool = world.GetPool<Translation>();
            var heroCharacterView = world.GetPool<CharacterView>();

            foreach(var e in enemies)
            {
                ref var translation = ref translationPool.Get(e);

                foreach(var h in heroes)
                {
                    ref var heroTR = ref heroCharacterView.Get(h).ViewTransform;

                    var sqDist = (translation.Value.position - heroTR.position).sqrMagnitude;
                    var sqRadius = ConstPrm.Enemy.VIEW_TARGET_RADIUS * ConstPrm.Enemy.VIEW_TARGET_RADIUS;
                    
                    if (sqDist <= sqRadius)
                    {
                        ref var targetComp = ref enemyTargetPool.Add(e);
                        targetComp.MyTarget = heroTR;
                        Util.Debug.PrintColor($"{translation.Value.name} => I see target {heroTR.name}", UnityEngine.Color.yellow);
                    }
                }
            }
        } 
    }
}