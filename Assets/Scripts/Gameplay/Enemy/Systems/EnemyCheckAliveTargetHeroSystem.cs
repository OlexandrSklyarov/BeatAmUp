using Leopotam.EcsLite;

namespace BT
{
    public sealed class EnemyCheckAliveTargetHeroSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            
            var enemies = world
                .Filter<Enemy>()
                .Inc<EnemyTarget>()
                .End();

            var heroes = world
                .Filter<Hero>()
                .Inc<CharacterView>()
                .Inc<Death>()
                .End();

            var enemyTargetPool = world.GetPool<EnemyTarget>();
            var viewPool = world.GetPool<CharacterView>();
            var attackStatePool = world.GetPool<AttackState>();

            foreach(var e in enemies)
            {
                ref var target = ref enemyTargetPool.Get(e);

                foreach(var h in heroes)
                {
                    ref var heroView = ref viewPool.Get(h);

                    if (IsMyTargetDeath(ref target, ref heroView))
                    {
                        enemyTargetPool.Del(e);

                        if (attackStatePool.Has(e)) attackStatePool.Del(e);

                        break;
                    }
                }
            }
        }


        private bool IsMyTargetDeath(ref EnemyTarget target, ref CharacterView heroView)
        {
            return target.MyTarget == heroView.ViewTransform;
        }
    }
}