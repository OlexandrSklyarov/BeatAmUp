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
                .Inc<MovementCommand>()
                .Inc<CombatCommand>()
                .Inc<CharacterGrounded>()
                .Inc<CharacterAttack>()
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

            var commandPool = world.GetPool<MovementCommand>();
            var combatCommandPool = world.GetPool<CombatCommand>();
            var heroPool = world.GetPool<Hero>();
            var attackPool = world.GetPool<CharacterAttack>();
            var translationPool = world.GetPool<Translation>();
            var viewPool = world.GetPool<CharacterView>();

            foreach (var heroEnt in heroes)
            {
                ref var command = ref commandPool.Get(heroEnt);
                ref var combat = ref combatCommandPool.Get(heroEnt);
                ref var attack = ref attackPool.Get(heroEnt);
                ref var hero = ref heroPool.Get(heroEnt);

                if (!combat.IsPunch && !combat.IsKick) return;

                ref var heroTranslation = ref translationPool.Get(heroEnt);
                ref var heroView = ref viewPool.Get(heroEnt);

                foreach (var enemy in enemies)
                {
                    ref var targetTranslation = ref translationPool.Get(enemy);

                    if (IsCanAttackTarget(ref heroView, ref heroTranslation, ref targetTranslation))
                    {
                        TryAddFinishAttack(world, enemy, ref hero, ref attack);
                        TryExecutePowerDamage(ref attack);

                        break;
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
            var maxDist = heroView.BodyRadius * 3f;

            if (sqDist > maxDist * maxDist) return false;

            return true;
        }


        private void TryAddFinishAttack(EcsWorld world, int enemyEntity, ref Hero hero, ref CharacterAttack attack)
        {
            var hpPool = world.GetPool<Health>();

            if (hpPool.Has(enemyEntity))
            {
                ref var enemyHealth = ref hpPool.Get(enemyEntity);
                attack.IsNeedFinishAttack = enemyHealth.CurrentHP <= hero.Data.Attack.MaxDamage;
            }
        }


        private void TryExecutePowerDamage(ref CharacterAttack attack)
        {
            if (attack.HitCount < ConstPrm.Character.MAX_HIT_COUNT) return;

            attack.IsPowerfulDamage = true;
        }
    }
}
