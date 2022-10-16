using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;

namespace Gameplay.Test
{
    public sealed class TestFixedUpdateSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            Util.Debug.Print($"run fixed update system...");

        }
    }
}