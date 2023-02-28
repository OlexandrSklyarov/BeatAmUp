using System;
using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class HeroMoveToNearestEnemySystem : IEcsRunSystem
    {
        public object ANGLE { get; private set; }

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var heroes = world.Filter<HeroTag>()
                .Inc<CharacterCommand>()
                .Inc<Movement>()
                .Inc<CharacterView>()
                .Inc<CharacterGrounded>()
                .Exc<Stun>()
                .Exc<Death>()
                .End();

            var enemies = world.Filter<Enemy>()
                .Inc<MovementAI>()
                .Exc<Death>()
                .End();

            var commandPool = world.GetPool<CharacterCommand>();
            var movementPool = world.GetPool<Movement>();
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

                        if (TrySlideToNearestTarget(ref heroView, ref heroMovement, ref enemyMovement))
                            break;
                    }
                }
            }
        }

        private bool TrySlideToNearestTarget(ref CharacterView heroView, ref Movement heroMovement, ref MovementAI enemyMovement)
        {
            var heroPos = heroMovement.Transform.position;
            var enemyPos = enemyMovement.MyTransform.position;
            enemyPos.y = heroPos.y;

            var toTarget = enemyPos - heroPos;
            var sqDist = toTarget.sqrMagnitude;

            if (Mathf.Abs(heroPos.y - enemyPos.y) > heroView.Height) return false;
            if (sqDist > ConstPrm.Hero.MAX_DIST_TO_ENEMY * ConstPrm.Hero.MAX_DIST_TO_ENEMY) return false;
            if (Vector3.Angle(toTarget, heroView.ViewTransform.forward) > ConstPrm.Hero.VIEW_ENEMY_ANGLE) return false;

            if (sqDist > ConstPrm.Hero.MIN_DIST_TO_ENEMY * ConstPrm.Hero.MIN_DIST_TO_ENEMY)
            {
                var targetPoint = heroPos + toTarget * (toTarget.magnitude - ConstPrm.Hero.MIN_DIST_TO_ENEMY);
                var end = Vector3.MoveTowards(heroPos, targetPoint, 1f);
                heroMovement.Transform.LeanMove(end, ConstPrm.Hero.SLIDE_TO_TARGET_TIME);
            };

            heroView.ViewTransform.rotation = Util.Vector3Math.DirToQuaternion(toTarget);

            return true;
        }
    }
}