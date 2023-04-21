using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class HeroRotateViewSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var config = systems.GetShared<SharedData>().Config;

            var entities = world.Filter<HeroTag>()
                .Inc<CharacterCommand>()
                .Inc<CharacterView>()
                .Inc<CharacterControllerMovement>()
                .Exc<CharacterSitDown>()
                .End();
                
            var inputPool = world.GetPool<CharacterCommand>();
            var viewPool = world.GetPool<CharacterView>();
            var movementPool = world.GetPool<CharacterControllerMovement>();

            foreach (var e in entities)
            {
                ref var input = ref inputPool.Get(e);
                ref var view = ref viewPool.Get(e);
                ref var move = ref movementPool.Get(e);

                if (!input.IsMoved) continue;

                view.ViewTransform.rotation = Quaternion.RotateTowards
                (
                    view.ViewTransform.rotation,
                    Util.Vector3Math.DirToQuaternion(move.HorizontalVelocity),
                    Time.deltaTime * config.PlayerData.RotateSpeed
                );
            }
        }
    }
}