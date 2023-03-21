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
                .Inc<CharacterControllerMovement>()
                .Inc<CharacterView>()
                .Inc<CharacterGrounded>()
                .Exc<HeroSlideTag>()
                .Exc<CharacterSitDown>()
                .Exc<Stun>()
                .Exc<Death>()
                .End();

            var enemies = world.Filter<Enemy>()
                .Inc<MovementAI>()
                .Inc<CharacterView>()
                .Exc<Death>()
                .End();

            var attackPool = world.GetPool<HeroAttack>();
            var commandPool = world.GetPool<CharacterCommand>();
            var movementPool = world.GetPool<CharacterControllerMovement>();
            var viewPool = world.GetPool<CharacterView>();
            var movementAIPool = world.GetPool<MovementAI>();            

            foreach (var hero in heroes)
            {
                ref var command = ref commandPool.Get(hero);

                if (command.IsPunch || command.IsKick)
                {
                    ref var heroAttack = ref attackPool.Get(hero);
                    ref var heroMovement = ref movementPool.Get(hero);
                    ref var heroView = ref viewPool.Get(hero);

                    foreach (var enemy in enemies)
                    {
                        ref var enemyMovement = ref movementAIPool.Get(enemy);
                        ref var enemyView = ref viewPool.Get(enemy);

                        if (TrySlideToNearestTarget(world, enemyMovement.MyTransform.position, 
                            enemyView.BodyRadius, hero, ref heroView, ref heroMovement))
                        {
                            TryAddFinishAttack(world, enemy, config.HeroAttackData.MaxDamage, ref heroAttack);

                            break;
                        }
                    }
                }
            }
        }


        private void TryAddFinishAttack(EcsWorld world, int enemy, int damageThreshold, ref HeroAttack heroAttack)
        {
            var hpPool = world.GetPool<Health>();
            if (hpPool.Has(enemy))
            {
                ref var enemyHP = ref hpPool.Get(enemy);
                heroAttack.IsNeedFinishAttack = enemyHP.HP <= damageThreshold;
            }

            var hitCounterPool = world.GetPool<HitCounter>();
            if (hitCounterPool.Has(enemy))
            {
                ref var counter = ref hitCounterPool.Get(enemy);
                heroAttack.IsCanThrowBackOpponent = counter.HitCount > ConstPrm.Character.MAX_HIT_COUNT;
            }            
        }


        private bool TrySlideToNearestTarget(EcsWorld world, Vector3 targetPos, float targetBodyRadius, int heroEntity, 
            ref CharacterView heroView, ref CharacterControllerMovement heroMovement)
        {
            var heroPos = heroMovement.Transform.position;
            targetPos.y = heroPos.y;

            if (Mathf.Abs(heroPos.y - targetPos.y) > heroView.Height) return false;
            
            var targetDir = targetPos - heroPos;
            var sqDist = targetDir.sqrMagnitude;
            var maxDist = targetBodyRadius + heroView.BodyRadius * ConstPrm.Hero.ATTACK_RADIUS_MULTIPLIER;

            if (sqDist > maxDist * maxDist) return false;
            if (Vector3.Angle(targetDir, heroView.ViewTransform.forward) > ConstPrm.Hero.VIEW_ENEMY_ANGLE) return false;

            SlideToTarget(targetPos, targetDir, world, heroEntity, targetBodyRadius, sqDist, ref heroView, ref heroMovement);

            return true;
        }


        private void SlideToTarget(Vector3 targetPos, Vector3 targetDir, 
            EcsWorld world, int heroEntity, float targetRadius, float sqDist,
            ref CharacterView heroView, ref CharacterControllerMovement heroMovement)
        {
            var minDistToTarget = targetRadius + heroView.BodyRadius;

            if (sqDist > minDistToTarget * minDistToTarget)
            {
                var end = targetPos - targetDir.normalized * minDistToTarget;
                heroMovement.Transform.LeanMove(end, ConstPrm.Hero.SLIDE_TO_TARGET_TIME);                    
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