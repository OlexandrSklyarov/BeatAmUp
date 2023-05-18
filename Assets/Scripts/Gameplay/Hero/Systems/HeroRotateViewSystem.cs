using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class HeroRotateViewSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var entities = world.Filter<Hero>()
                .Inc<MovementCommand>()
                .Inc<CharacterView>()
                .Inc<CharacterControllerMovement>()
                .Exc<CharacterSitDown>()
                .End();
                
            var heroPool = world.GetPool<Hero>();
            var inputPool = world.GetPool<MovementCommand>();
            var viewPool = world.GetPool<CharacterView>();
            var movementPool = world.GetPool<CharacterControllerMovement>();

            foreach (var e in entities)
            {
                ref var hero = ref heroPool.Get(e);
                ref var input = ref inputPool.Get(e);
                ref var view = ref viewPool.Get(e);
                ref var move = ref movementPool.Get(e);

                if (!input.IsMoved) continue;

                view.ViewTransform.rotation = Quaternion.RotateTowards
                (
                    view.ViewTransform.rotation,
                    Util.Vector3Math.DirToQuaternion(move.HorizontalVelocity),
                    Time.deltaTime * hero.Data.RotateSpeed
                );
            }
        }
    }
}