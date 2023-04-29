using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class HeroFindNearEnemyTargetSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var heroes = world
                .Filter<Hero>()
                .Inc<CharacterCommand>()
                .Inc<Translation>()
                .Inc<CharacterView>()
                .Inc<CharacterGrounded>()
                .Exc<HeroSlideToTarget>()
                .Exc<CharacterSitDown>()
                .Exc<Stun>()
                .Exc<Death>()
                .End();

            var enemies = world
                .Filter<Enemy>()
                .Inc<Translation>()
                .Inc<CharacterView>()
                .Exc<Death>()
                .End();

            var commandPool = world.GetPool<CharacterCommand>();
            var translationPool = world.GetPool<Translation>();
            var viewPool = world.GetPool<CharacterView>();             
            var slidePool = world.GetPool<HeroSlideToTarget>();            

            foreach (var heroEnt in heroes)
            {
                ref var command = ref commandPool.Get(heroEnt);
                ref var heroTranslation = ref translationPool.Get(heroEnt);
                ref var heroView = ref viewPool.Get(heroEnt);

                if (!command.IsRunning) continue;
                if (!command.IsPunch && !command.IsKick) continue;

                foreach (var enemy in enemies)
                {
                    ref var enemyTranslation = ref translationPool.Get(enemy);
                    ref var enemyView = ref viewPool.Get(enemy);

                    if (TrySlideToNearestTarget(ref heroTranslation, ref heroView, ref enemyTranslation))
                    {
                        AddSlideState(slidePool, heroEnt, ref enemyView, ref enemyTranslation);

                        break;
                    }
                }
            }
        }

        
        private void AddSlideState(EcsPool<HeroSlideToTarget> slidePool, int hero, ref CharacterView targetView,  
            ref Translation targetTranslation)
        {
            var r = targetView.BodyRadius + ConstPrm.Hero.TARGET_RADIUS_OFFSET;

            ref var slide = ref slidePool.Add(hero);
            slide.TargetPosition = targetTranslation.Value.position;
            slide.TargetSqBodyRadius = r * r;
        }


        private bool TrySlideToNearestTarget(ref Translation heroTR, ref CharacterView heroView, ref Translation targetTR)
        {
            if (Mathf.Abs(heroTR.Value.position.y - targetTR.Value.position.y) > heroView.Height) return false;

            var toTarget = targetTR.Value.position - heroTR.Value.position;
            
            if (Vector3.Angle(toTarget, heroView.ViewTransform.forward) > ConstPrm.Hero.VIEW_ENEMY_ANGLE) return false;
            
            var sqDist = toTarget.sqrMagnitude;
            var minDist = heroView.BodyRadius * 2f;
            var maxDist = heroView.BodyRadius * ConstPrm.Hero.ATTACK_RADIUS_MULTIPLIER;

            if (sqDist > maxDist * maxDist ||  sqDist <= minDist) return false;

            return true;
        }
    }
}