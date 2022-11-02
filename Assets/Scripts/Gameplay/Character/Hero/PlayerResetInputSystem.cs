using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;

namespace Gameplay.Character.Hero
{
    public class PlayerResetInputSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var entities = systems.GetWorld()
                .Filter<PlayerInputData>()
                .Inc<Movement>()
                .Inc<HeroTag>()
                .End();

            var inputDataPool = systems.GetWorld().GetPool<PlayerInputData>();

            foreach(var e in entities)
            {
                ref var input = ref inputDataPool.Get(e); 
                input.IsJump = false;
            }
        }
    }
}