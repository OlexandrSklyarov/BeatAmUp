using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class HeroFindNearEnemyTargetSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var config = systems.GetShared<SharedData>().Config;

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

            foreach (var hero in heroes)
            {
                ref var command = ref commandPool.Get(hero);

                if (command.IsPunch || command.IsKick)
                {
                    ref var heroTranslation = ref translationPool.Get(hero);
                    ref var heroView = ref viewPool.Get(hero);

                    foreach (var enemy in enemies)
                    {
                        ref var targetTranslation = ref translationPool.Get(enemy);
                        ref var targetView = ref viewPool.Get(enemy);

                        if (TrySlideToNearestTarget(ref heroTranslation, ref heroView, ref targetTranslation))
                        {
                            var r = targetView.BodyRadius + ConstPrm.Hero.TARGET_RADIUS_OFFSET;
                            
                            ref var slide = ref slidePool.Add(hero);
                            slide.TargetPosition = targetTranslation.Value.position;
                            slide.TargetSqBodyRadius = r * r;
                            
                            break;
                        }
                    }
                }
            }
        }

        
        private bool TrySlideToNearestTarget(ref Translation heroTR, ref CharacterView heroView, ref Translation targetTR)
        {
            if (Mathf.Abs(heroTR.Value.position.y - targetTR.Value.position.y) > heroView.Height) return false;

            var toTarget = targetTR.Value.position - heroTR.Value.position;
            var sqDist = toTarget.sqrMagnitude;
            var minDist = heroView.BodyRadius * 2f;
            var maxDist = heroView.BodyRadius * ConstPrm.Hero.ATTACK_RADIUS_MULTIPLIER;

            if (sqDist > maxDist * maxDist ||  sqDist <= minDist) return false;
            
            if (Vector3.Angle(toTarget, heroView.ViewTransform.forward) > ConstPrm.Hero.VIEW_ENEMY_ANGLE) return false;
            
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
    }
}