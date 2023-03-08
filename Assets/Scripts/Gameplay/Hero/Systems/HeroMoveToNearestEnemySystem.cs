using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class HeroMoveToNearestEnemySystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var heroes = world.Filter<HeroTag>()
                .Inc<CharacterCommand>()
                .Inc<CharacterControllerMovement>()
                .Inc<CharacterView>()
                .Inc<CharacterGrounded>()
                .Exc<HeroSlideTag>()
                .Exc<Stun>()
                .Exc<Death>()
                .End();

            var enemies = world.Filter<Enemy>()
                .Inc<MovementAI>()
                .Inc<CharacterView>()
                .Exc<Death>()
                .End();

            var commandPool = world.GetPool<CharacterCommand>();
            var movementPool = world.GetPool<CharacterControllerMovement>();
            var viewPool = world.GetPool<CharacterView>();
            var movementAIPool = world.GetPool<MovementAI>();

            foreach (var h in heroes)
            {
                ref var command = ref commandPool.Get(h);
                ref var heroMovement = ref movementPool.Get(h);
                ref var heroView = ref viewPool.Get(h);

                if (command.IsPunch || command.IsKick)
                {
                    foreach (var e in enemies)
                    {
                        ref var enemyMovement = ref movementAIPool.Get(e);
                        ref var enemyView = ref viewPool.Get(e);

                        if (TrySlideToNearestTarget(h, ref heroView, ref heroMovement, world,
                            enemyMovement.MyTransform.position, enemyView.BodyRadius))
                            break;
                    }
                }
            }
        }


        private bool TrySlideToNearestTarget(int heroEntity, ref CharacterView heroView, ref CharacterControllerMovement heroMovement, EcsWorld world,
            Vector3 targetPos, float targetRadius)
        {
            var heroPos = heroMovement.Transform.position;
            targetPos.y = heroPos.y;

            if (Mathf.Abs(heroPos.y - targetPos.y) > heroView.Height) return false;
            
            var toTarget = targetPos - heroPos;
            var sqDist = toTarget.sqrMagnitude;
            var maxDist = targetRadius + heroView.BodyRadius * ConstPrm.Hero.ATTACK_RADIUS_MULTIPLIER;

            if (sqDist > maxDist * maxDist) return false;
            if (Vector3.Angle(toTarget, heroView.ViewTransform.forward) > ConstPrm.Hero.VIEW_ENEMY_ANGLE) return false;

            var minDistToTarget = targetRadius + heroView.BodyRadius;

            if (sqDist > minDistToTarget * minDistToTarget)
            {
                var end = targetPos - toTarget.normalized * minDistToTarget;
                heroMovement.Transform.LeanMove(end, ConstPrm.Hero.SLIDE_TO_TARGET_TIME);                    
            };

            var slidePool = world.GetPool<HeroSlideTag>();
            slidePool.Add(heroEntity);

            var angle = Util.Vector3Math.GetUpAxisAngleRotate(toTarget);
            heroView.ViewTransform
                .LeanRotateY(angle, ConstPrm.Hero.SLIDE_TO_TARGET_TIME)
                .setOnComplete(() => slidePool.Del(heroEntity));

            return true;
        }
    }
}