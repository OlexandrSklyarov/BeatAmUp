using Leopotam.EcsLite;
using Services.Data;
using UnityEngine;

namespace Gameplay.Character
{
    public class CheckGroundSystem : IEcsRunSystem
    {
        private RaycastHit[] _hits = new RaycastHit[1];
        

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var config = systems.GetShared<SharedData>().Config;

            var entities = world.Filter<Movement>().End();
            var movementPool = world.GetPool<Movement>();
           
            foreach(var e in entities)
            {
                ref var movement = ref movementPool.Get(e); 

                var ray = new Ray(movement.Transform.position, Vector3.down);
                
                movement.IsGround = Physics
                    .RaycastNonAlloc(ray, _hits, config.CharacterData.CheckGroundDistance) > 0; 
            }
        }
    }
}
