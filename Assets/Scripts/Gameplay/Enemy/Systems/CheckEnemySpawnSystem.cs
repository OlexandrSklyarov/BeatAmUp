using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public class CheckEnemySpawnSystem : IEcsInitSystem, IEcsRunSystem
    {
        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var spawnerPool = world.GetPool<EnemySpawner>();

            var entity = world.NewEntity();
            ref var spawner = ref spawnerPool.Add(entity);
            spawner.SpawnPointHolders = UnityEngine.Object.FindObjectsOfType<EnemySpawnPointHolder>();
        }


        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var filter = world.Filter<EnemySpawner>().End();
            var heroes = world.Filter<Hero>().Inc<Translation>().End();
            var enemies = world.Filter<Enemy>().Inc<Translation>().End();

            var spawnerPool = world.GetPool<EnemySpawner>();
            var translationPool = world.GetPool<Translation>();
            var createEventPool = world.GetPool<CreateEnemyEvent>();

            var heroesCount = heroes.GetEntitiesCount();
            if (heroesCount <= 0) return;

            var maxCount = ConstPrm.Enemy.MAX_ENEMY_ON_LEVEL * heroesCount;
            var enemyCount = enemies.GetEntitiesCount();
            if (enemyCount >= maxCount) return;

            var needCreateAmount = maxCount - enemyCount;

            foreach (var ent in filter)
            {
                ref var spawner = ref spawnerPool.Get(ent);

                if (spawner.SpawnTimer > 0f)
                {
                    spawner.SpawnTimer -= Time.deltaTime;
                    continue;
                }

                foreach (var hero in heroes)
                {
                    ref var heroTR = ref translationPool.Get(hero);

                    foreach (var point in spawner.SpawnPointHolders[0].GetSpawnPoints())
                    {
                        if ((point.position - heroTR.Value.position).sqrMagnitude > 20f * 20f) continue;

                        var entity = world.NewEntity();

                        ref var createEvent = ref createEventPool.Add(entity);
                        createEvent.Type = EnemyType.TestKnight;
                        createEvent.CreatePosition = point.position;
                        createEvent.CreateRotation = point.rotation;                        

                        if (--needCreateAmount <= 0)
                        {
                            spawner.SpawnTimer = ConstPrm.Enemy.CHECK_SPAWN_TIME;
                            return;
                        }
                    }
                }
            }
        }
    }
}