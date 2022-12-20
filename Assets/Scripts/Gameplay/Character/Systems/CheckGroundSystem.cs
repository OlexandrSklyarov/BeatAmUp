using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class CheckGroundSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var config = systems.GetShared<SharedData>().Config;

            var entities = world
                .Filter<Movement>()
                .End();

            var movementPool = world.GetPool<Movement>();
            var groundedPool = world.GetPool<CharacterGrounded>();

            foreach(var e in entities)
            {
                ref var movement = ref movementPool.Get(e);

                var isGround = Physics.CheckSphere
                (
                    movement.Transform.position + Vector3.up * config.CharacterData.CheckGroundRadius * 0.5f,
                    config.CharacterData.CheckGroundRadius,
                    config.CharacterData.GroundLayer
                );

                var isHasGrounded = groundedPool.Has(e);

                if (isGround)
                {
                    if (!isHasGrounded) 
                    {
                        Util.Debug.Print("Add ground");
                        groundedPool.Add(e);
                    }
                }   
                else
                {
                    if (isHasGrounded) 
                    {
                        Util.Debug.Print("Del ground");
                        groundedPool.Del(e);
                    }
                }  
            }
        }
    }
}