using System.Collections.Generic;
using Gameplay.Factories;

namespace BT
{
    public sealed class EnemyFactory
    {
        private readonly Dictionary<EnemyType, EntityFactory<EnemyViewProvider>> _factories;

        public EnemyFactory(EnemyPoolData[] poolData)
        {
            _factories = new Dictionary<EnemyType, EntityFactory<EnemyViewProvider>>();

            for(int i = 0; i < poolData.Length; i++)
            {
                var data = poolData[i];
                var factory = new EntityFactory<EnemyViewProvider>(data.Prefab, data.PoolSize);

                _factories.Add(data.Type, factory);
            }
        }


        public EnemyViewProvider GetEnemyView(EnemyType type)
        {
            return _factories[type].GetItem((enemy, storage) =>
            {
                enemy.Init(storage);
            });
        }
    }
}