using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Services.Data;
using UnityEngine;

namespace Gameplay.Character.Hero
{
    public sealed class InitPlayerSystem : IEcsInitSystem
    {
        private readonly EcsCustomInject<WorldData>  _worldData = default;
        private readonly EcsCustomInject<GameConfig> _gameConfig = default;


        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var heroView = UnityEngine.Object.Instantiate
            (
                _gameConfig.Value.PlayerData.Prefab, 
                _worldData.Value.HeroSpawnPoint.position, 
                UnityEngine.Quaternion.identity
            );

            var hero = world.NewEntity();
            
            var heroTagPool =  world.GetPool<HeroTag>();
            heroTagPool.Add(hero);

            var movementPool =  world.GetPool<Movement>();
            ref var movement = ref movementPool.Add(hero);

            movement.Body = heroView.GetComponent<Rigidbody>();
            movement.Transform = heroView.transform;
            movement.Speed = _gameConfig.Value.PlayerData.Speed;

            Util.Debug.Print("hero init...");
        }
    }
}