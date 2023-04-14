using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class HeroSittingSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world  = systems.GetWorld();

            var entities = world
                .Filter<Hero>()
                .Inc<CharacterCommand>()
                .End();

            var characterCommandPool = world.GetPool<CharacterCommand>();
            var sitingPool = world.GetPool<CharacterSitDown>();
            var groundedPool = world.GetPool<CharacterGrounded>();

            foreach(var e in entities)
            {
                ref var command = ref characterCommandPool.Get(e);                

                var isSittingState = sitingPool.Has(e);
                var isGrounded = groundedPool.Has(e);

                if (!isGrounded)
                {
                    if (isSittingState) sitingPool.Del(e);                                        
                    continue;
                }

                if (command.IsSitting)
                {
                    if (!isSittingState)
                    {
                        sitingPool.Add(e);
                        Util.Debug.PrintColor("Add CharacterSitDown comp", Color.yellow);
                    }
                }   
                else
                {
                    if (isSittingState) sitingPool.Del(e);
                }  
            }
        }
    }
}