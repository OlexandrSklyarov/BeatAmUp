using Leopotam.EcsLite;

namespace BT
{
    public class InitSpawnEnemyZoneSystem : IEcsInitSystem
    {
        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            
            var eventPool = world.GetPool<CreateEnemyEvent>();

            var holders = UnityEngine.Object.FindObjectsOfType<EnemySpawnPointHolder>();

            foreach(var point in holders[0].GetSpawnPoints())
            {
                var entity = world.NewEntity();

                ref var createEvent = ref eventPool.Add(entity);
                createEvent.Type = EnemyType.TestKnight;
                createEvent.CreatePosition = point.position;
                createEvent.CreateRotation = point.rotation;
            }

            UnityEngine.Object.Destroy(holders[0].gameObject);
        }        
    }
}