using Leopotam.EcsLite;

namespace BT
{
    public sealed class TestFixedUpdateSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            Util.Debug.Print($"run fixed update system...");

        }
    }
}