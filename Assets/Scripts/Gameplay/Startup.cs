using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class Startup : MonoBehaviour
    {        
        [SerializeField] private WorldData _worldData;
        [Space(10f), SerializeField] private GameConfig _gameConfig;

        private EcsWorld _world;
        private IEcsSystems _initSystems;
        private IEcsSystems _updateSystems;
        private IEcsSystems _fixedUpdateSystems;
        private IEcsSystems _lateUpdateSystems;

        private void Start()
        {
            _world = new EcsWorld();

            var data = new SharedData()
            {
                InputProvider = new InputHandleProvider(new InputServices()),
                Config = _gameConfig,
                WorldData = _worldData
            };
            
            _initSystems = new EcsSystems(_world, data);
            _updateSystems = new EcsSystems(_world, data);
            _fixedUpdateSystems = new EcsSystems(_world, data);
            _lateUpdateSystems = new EcsSystems(_world, data);

            AddInitSystems();
            AddSystems();       
        }


        private void AddInitSystems()
        {
            _initSystems                
                .Add(new InitWorldSystem())
                .Add(new InitHeroSystem())
                .Add(new InitCameraSystem())
                .Init();
        }


        private void AddSystems()
        {
            _updateSystems
                .Add(new CheckGroundSystem())
                .Add(new ApplyGravitySystem())
                .Add(new PlayerInputSystem())
                .Add(new PlayerSittingSystem())
                .Add(new HeroAttackSystem())
                .Add(new HeroJumpSystem())
                .Add(new CharacterRotateViewSystem())
                .Add(new HeroChangeHorizontalVelocitySystem())
                .Add(new StopMovementWhenAttackingSystem())
                .Add(new ApplyHorizontalVelocitySystem())
                .Add(new HeroAnimationSystem())
                .Init();
            
            _fixedUpdateSystems
                .Init();

            _lateUpdateSystems
                .Add(new ResetPlayerInputDataSystem())
                .Init();
        }


        private void Update() => _updateSystems?.Run();
        
        
        private void FixedUpdate() => _fixedUpdateSystems?.Run();


        private void LateUpdate() => _lateUpdateSystems?.Run();


        private void OnDestroy()
        {
            _initSystems?.Destroy();
            _initSystems = null;

            _updateSystems?.Destroy();
            _updateSystems = null;

            _lateUpdateSystems?.Destroy();
            _lateUpdateSystems = null;

            _fixedUpdateSystems?.Destroy();
            _fixedUpdateSystems = null;

            _world?.Destroy();
            _world = null;
        }
    }
}