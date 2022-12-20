using Leopotam.EcsLite;

namespace BT
{
    public sealed class TestRunSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            Util.Debug.Print($"run world... systems {systems.ToString()}");
        }
    }
}