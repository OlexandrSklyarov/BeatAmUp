using Leopotam.EcsLite;

namespace Gameplay.Test
{
    public sealed class TestRunSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            Util.Debug.Print($"run world... systems {systems.ToString()}");
        }
    }
}