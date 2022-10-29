using Leopotam.EcsLite;
using UnityEngine;

namespace Gameplay.Character
{
    public class CheckGroundSystem : IEcsRunSystem
    {
        private RaycastHit[] _hits = new RaycastHit[1];

        private const float MAX_DIST = 0.1f;


        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var entities = world.Filter<Movement>().End();
            var movementPool = world.GetPool<Movement>();
           
            foreach(var e in entities)
            {
                ref var movement = ref movementPool.Get(e); 

                var ray = new Ray(movement.Transform.position, Vector3.down);
                movement.IsGround = Physics.RaycastNonAlloc(ray, _hits, MAX_DIST) > 0; 
            }
        }
    }
}
