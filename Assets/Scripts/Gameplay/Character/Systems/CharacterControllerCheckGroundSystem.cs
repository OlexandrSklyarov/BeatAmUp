using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class CharacterControllerCheckGroundSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var config = systems.GetShared<SharedData>().Config;

            var entities = world
                .Filter<Translation>()
                .End();

            var translationPool = world.GetPool<Translation>();
            var groundedPool = world.GetPool<CharacterGrounded>();

            foreach(var e in entities)
            {
                ref var translation = ref translationPool.Get(e);

                var isGroundCollision = Physics.CheckSphere
                (
                    translation.Value.position + Vector3.up * config.CharacterData.CheckGroundRadius * 0.5f,
                    config.CharacterData.CheckGroundRadius,
                    config.CharacterData.GroundLayer
                );

                var isHasGrounded = groundedPool.Has(e);

                if (isGroundCollision)
                {
                    if (!isHasGrounded) 
                    {
                        groundedPool.Add(e);                        
                    }
                }   
                else
                {
                    if (isHasGrounded) 
                    {
                        groundedPool.Del(e);
                    }
                }  
            }
        }
    }
}