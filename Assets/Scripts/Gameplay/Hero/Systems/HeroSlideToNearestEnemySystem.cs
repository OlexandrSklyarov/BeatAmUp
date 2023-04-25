using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class HeroSlideToNearestEnemySystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var config = systems.GetShared<SharedData>().Config;

            var heroes = world.Filter<HeroTag>()
                .Inc<HeroAttack>()
                .Inc<CharacterCommand>()
                .Inc<Translation>()
                .Inc<CharacterView>()
                .Inc<CharacterGrounded>()
                .Exc<HeroSlideTag>()
                .Exc<CharacterSitDown>()
                .Exc<Stun>()
                .Exc<Death>()
                .End();

            var enemies = world.Filter<Enemy>()
                .Inc<Translation>()
                .Inc<CharacterView>()
                .Exc<Death>()
                .End();

            var attackPool = world.GetPool<HeroAttack>();
            var commandPool = world.GetPool<CharacterCommand>();
            var translationPool = world.GetPool<Translation>();
            var viewPool = world.GetPool<CharacterView>();            

            foreach (var hero in heroes)
            {
                ref var command = ref commandPool.Get(hero);

                if (command.IsPunch || command.IsKick)
                {
                    ref var heroAttack = ref attackPool.Get(hero);
                    ref var heroTranslation = ref translationPool.Get(hero);
                    ref var heroView = ref viewPool.Get(hero);

                    foreach (var enemy in enemies)
                    {
                        ref var translation = ref translationPool.Get(enemy);
                        ref var enemyView = ref viewPool.Get(enemy);

                        if (TrySlideToNearestTarget(world, translation.Value.position, 
                            enemyView.BodyRadius, hero, ref heroView, ref heroTranslation))
                        {
                            TryAddFinishAttack(world, enemy, config.HeroAttackData.MaxDamage, ref heroAttack);

                            break;
                        }
                    }
                }
            }
        }

        
        private bool TrySlideToNearestTarget(EcsWorld world, Vector3 targetPos, float targetBodyRadius, int heroEntity, 
            ref CharacterView heroView, ref Translation heroTranslation)
        {
            var heroPos = heroTranslation.Value.position;
            targetPos.y = heroPos.y;

            if (Mathf.Abs(heroPos.y - targetPos.y) > heroView.Height) return false;
            
            var targetDir = targetPos - heroPos;
            var sqDist = targetDir.sqrMagnitude;
            var maxDist = targetBodyRadius + heroView.BodyRadius * ConstPrm.Hero.ATTACK_RADIUS_MULTIPLIER;

            if (sqDist > maxDist * maxDist) return false;
            if (Vector3.Angle(targetDir, heroView.ViewTransform.forward) > ConstPrm.Hero.VIEW_ENEMY_ANGLE) return false;

            SlideToTarget(targetPos, targetDir, world, heroEntity, targetBodyRadius, sqDist, ref heroView, ref heroTranslation);

            return true;
        }
        

        private void TryAddFinishAttack(EcsWorld world, int enemy, int damageThreshold, ref HeroAttack heroAttack)
        {
            var hpPool = world.GetPool<Health>();
            if (hpPool.Has(enemy))
            {
                ref var enemyHP = ref hpPool.Get(enemy);
                heroAttack.IsNeedFinishAttack = enemyHP.CurrentHP <= damageThreshold;
            }

            var hitCounterPool = world.GetPool<HitCounter>();
            if (hitCounterPool.Has(enemy))
            {
                ref var counter = ref hitCounterPool.Get(enemy);
                heroAttack.IsPowerfulDamage = counter.HitCount > ConstPrm.Character.MAX_HIT_COUNT;
            }            
        }


        private void SlideToTarget(Vector3 targetPos, Vector3 targetDir, 
            EcsWorld world, int heroEntity, float targetRadius, float sqDist,
            ref CharacterView heroView, ref Translation heroTranslation)
        {
            var minDistToTarget = targetRadius + heroView.BodyRadius;

            if (sqDist > minDistToTarget * minDistToTarget)
            {
                var end = targetPos - targetDir.normalized * minDistToTarget;
                heroTranslation.Value.LeanMove(end, ConstPrm.Hero.SLIDE_TO_TARGET_TIME);                    
            };

            var slidePool = world.GetPool<HeroSlideTag>();
            slidePool.Add(heroEntity);

            var angle = Util.Vector3Math.GetUpAxisAngleRotate(targetDir);
            heroView.ViewTransform
                .LeanRotateY(angle, ConstPrm.Hero.SLIDE_TO_TARGET_TIME)
                .setOnComplete(() => slidePool.Del(heroEntity));
        }
    }
}