using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Services.Data;
using UnityEngine;

namespace Gameplay.Character.Hero
{
    public sealed class InitHeroSystem : IEcsInitSystem
    {
        private readonly EcsCustomInject<WorldData>  _worldData = default;
        private readonly EcsCustomInject<GameConfig> _gameConfig = default;        


        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var heroGO = Object.Instantiate
            (
                _gameConfig.Value.PlayerData.Prefab, 
                _worldData.Value.HeroSpawnPoint.position, 
                Quaternion.identity
            );

            var heroEntity = world.NewEntity();
            
            var heroTagPool =  world.GetPool<HeroTag>();
            heroTagPool.Add(heroEntity);

            var inputDataPool =  world.GetPool<PlayerInputData>();
            inputDataPool.Add(heroEntity);

            var movementPool =  world.GetPool<Movement>();
            ref var movement = ref movementPool.Add(heroEntity);
            movement.Body = heroGO.GetComponent<Rigidbody>();
            movement.Transform = heroGO.transform;
            movement.Speed = _gameConfig.Value.PlayerData.Speed;

            var viewPool = world.GetPool<CharacterView>();
            ref var view = ref viewPool.Add(heroEntity);
            view.Animator = heroGO.GetComponentInChildren<Animator>();
            
            Util.Debug.Print($"hero init...");
        }
    }
}