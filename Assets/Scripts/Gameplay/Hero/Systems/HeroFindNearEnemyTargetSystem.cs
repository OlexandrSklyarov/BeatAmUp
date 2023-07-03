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
                .Inc<MovementCommand>()
                .Inc<CombatCommand>()
                .Inc<Translation>()
                .Inc<CharacterView>()
                .Inc<CharacterGrounded>()
                .Exc<SlideToTargetProcess>()
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
            var translationPool = world.GetPool<Translation>();
            var viewPool = world.GetPool<CharacterView>();             
            var slidePool = world.GetPool<SlideToTargetProcess>();            

            foreach (var heroEnt in heroes)
            {
                ref var movementCommand = ref commandPool.Get(heroEnt);
                ref var combatCommand = ref combatCommandPool.Get(heroEnt);
                ref var heroTranslation = ref translationPool.Get(heroEnt);
                ref var heroView = ref viewPool.Get(heroEnt);

                if (!movementCommand.IsRunning) continue;
                if (!combatCommand.IsPunch && !combatCommand.IsKick) continue;

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

        
        private void AddSlideState(EcsPool<SlideToTargetProcess> slidePool, int hero, ref CharacterView targetView,  
            ref Translation targetTranslation)
        {
            ref var slide = ref slidePool.Add(hero);
            slide.TargetPosition = targetTranslation.Value.position;
            slide.TargetBodyRadius = targetView.BodyRadius + ConstPrm.Hero.TARGET_RADIUS_OFFSET;
        }


        private bool TrySlideToNearestTarget(ref Translation heroTR, ref CharacterView heroView, ref Translation targetTR)
        {
            if (Mathf.Abs(heroTR.Value.position.y - targetTR.Value.position.y) > heroView.Height) return false;

            var toTarget = targetTR.Value.position - heroTR.Value.position;
            
            if (Vector3.Angle(toTarget, heroView.ViewTransform.forward) > ConstPrm.Hero.VIEW_ENEMY_ANGLE) return false;
            
            var sqDist = toTarget.sqrMagnitude;
            var minDist = heroView.BodyRadius * 2f;
            var maxDist = heroView.BodyRadius * ConstPrm.Hero.ATTACK_RADIUS_MULTIPLIER;

            return (sqDist <= maxDist * maxDist && sqDist >= minDist * minDist);
        }
    }
}