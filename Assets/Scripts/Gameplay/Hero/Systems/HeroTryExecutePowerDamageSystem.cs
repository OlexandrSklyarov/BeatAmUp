using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public class HeroTryExecutePowerDamageSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var heroes = world
                .Filter<Hero>()
                .Inc<CharacterCommand>()
                .Inc<CharacterGrounded>()
                .Inc<HeroAttack>()
                .Inc<CharacterView>()
                .Inc<Translation>()
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
            var heroPool = world.GetPool<Hero>();            
            var attackPool = world.GetPool<HeroAttack>();
            var translationPool = world.GetPool<Translation>();
            var viewPool = world.GetPool<CharacterView>();   

            foreach (var heroEnt in heroes)
            {
                ref var command = ref commandPool.Get(heroEnt);
                ref var attack = ref attackPool.Get(heroEnt);
                ref var hero = ref heroPool.Get(heroEnt);
                
                if (command.IsPunch || command.IsKick)
                {
                    ref var heroTranslation = ref translationPool.Get(heroEnt);
                    ref var heroView = ref viewPool.Get(heroEnt);

                    foreach (var enemy in enemies)
                    {
                        ref var targetTranslation = ref translationPool.Get(enemy);

                        if (IsCanAttackTarget(ref heroView, ref heroTranslation, ref targetTranslation))
                        {
                            TryAddFinishAttack(world, enemy, ref hero, ref attack);
                            
                            break;
                        }
                    }
                }
            }
        }
        

        private bool IsCanAttackTarget(ref CharacterView heroView, ref Translation heroTR, ref Translation targetTR)
        {
            if (Mathf.Abs(heroTR.Value.position.y - targetTR.Value.position.y) > heroView.Height) return false;

            var toTarget = targetTR.Value.position - heroTR.Value.position;
            
            if (Vector3.Angle(toTarget, heroView.ViewTransform.forward) > ConstPrm.Hero.VIEW_ENEMY_ANGLE) return false;
            
            var sqDist = toTarget.sqrMagnitude;
            var maxDist = heroView.BodyRadius * 2f;

            if (sqDist > maxDist * maxDist) return false;

            return true;
        }


        private void TryAddFinishAttack(EcsWorld world, int enemyEntity, ref Hero hero, ref HeroAttack heroAttack)
        {
            var hpPool = world.GetPool<Health>();
            
            if (hpPool.Has(enemyEntity))
            {
                ref var enemyHP = ref hpPool.Get(enemyEntity);
                heroAttack.IsNeedFinishAttack = enemyHP.CurrentHP <= hero.Data.Attack.MaxDamage;
            }

            var hitCounterPool = world.GetPool<HitCounter>();
            if (hitCounterPool.Has(enemyEntity))
            {
                ref var counter = ref hitCounterPool.Get(enemyEntity);
                heroAttack.IsPowerfulDamage = counter.HitCount > ConstPrm.Character.MAX_HIT_COUNT;
            }            
        }
    }
}