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
                .Inc<MovementAI>()
                .Exc<EnemyTarget>()
                .End();

            var heroes = world.Filter<Hero>()
                .Inc<CharacterView>()
                .Exc<Death>()
                .End();

            var enemyTargetPool = world.GetPool<EnemyTarget>();
            var translationPool = world.GetPool<Translation>();
            var aiPool = world.GetPool<MovementAI>();
            var viewPool = world.GetPool<CharacterView>();

            foreach(var e in enemies)
            {
                ref var translation = ref translationPool.Get(e);
                ref var ai = ref aiPool.Get(e);

                foreach(var h in heroes)
                {
                    ref var heroView = ref viewPool.Get(h);

                    var sqDist = (translation.Value.position - heroView.ViewTransform.position).sqrMagnitude;
                    var sqRadius = ConstPrm.Enemy.VIEW_TARGET_RADIUS * ConstPrm.Enemy.VIEW_TARGET_RADIUS;
                    
                    if (sqDist <= sqRadius)
                    {
                        ref var target = ref enemyTargetPool.Add(e);
                        target.MyTarget = heroView.ViewTransform;
                        target.TargetRadius = heroView.BodyRadius;
                        target.MinVisualDistance = GetTargetVisualDistance(ref ai);
                    }
                }
            }
        }


        private float GetTargetVisualDistance(ref MovementAI ai)
        {
            return ai.NavAgent.radius + 
                ConstPrm.Enemy.TARGET_ENCIRCLEMENT_RADIUS * UnityEngine.Random.Range(0.65f, 1f);
        }
    }
}