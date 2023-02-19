using Gameplay.FX;
using Leopotam.EcsLite;
using UnityEngine;
using Util.Console;

namespace BT
{
    public sealed class Startup : MonoBehaviour
    {        
        [SerializeField] private WorldData _worldData;
        [Space(10f), SerializeField] private GameConfig _gameConfig;
        [Space(10f), SerializeField] private VfxData _vfxConfig;
        [Space(10f), SerializeField] private EnemyData _enemyConfig;

        private EcsWorld _world;
        private IEcsSystems _initSystems;
        private IEcsSystems _updateSystems;
        private IEcsSystems _fixedUpdateSystems;
        private IEcsSystems _lateUpdateSystems;

        private void Start()
        {   
            _world = new EcsWorld();

            var inputService = new InputServices();

            var data = new SharedData()
            {
                InputProvider = new InputHandleProvider(inputService),
                Config = _gameConfig,
                WorldData = _worldData,
                VFXController = new VisualFXController(_vfxConfig),
                EnemyFactory = new EnemyFactory(_enemyConfig)
            };
            
            _initSystems = new EcsSystems(_world, data);
            _updateSystems = new EcsSystems(_world, data);
            _fixedUpdateSystems = new EcsSystems(_world, data);
            _lateUpdateSystems = new EcsSystems(_world, data);

            AddInitSystems();
            AddSystems();      
            
            TryCreateDebugConsole(inputService); 
        }


        private void TryCreateDebugConsole(InputServices inputService)
        {
            #if DEVELOPMENT_BUILD || UNITY_EDITOR
            var console = new GameObject("[DEBUG_CONSOLE]").AddComponent<DebugConsole>();
            console.Init(inputService);       
            #endif
        }


        private void AddInitSystems()
        {
            _initSystems                
                .Add(new InitWorldSystem())
                .Add(new InitHeroSystem())
                .Add(new InitCameraSystem())
                .Add(new InitSpawnEnemyZoneSystem())
                .Add(new InitTestPunchBoxSystem())
                .Init();
        }


        private void AddSystems()
        {
            _updateSystems  
                .Add(new CheckGroundSystem())
                .Add(new ApplyGravitySystem())
                .Add(new PlayerInputSystem())
                .Add(new PlayerSittingSystem())
                .Add(new HeroComboAttackSystem())
                .Add(new HeroJumpSystem())
                .Add(new CharacterRotateViewSystem())
                .Add(new HeroChangeHorizontalVelocitySystem())
                .Add(new StopMovementWhenAttackingSystem())
                .Add(new ApplyHorizontalVelocitySystem())
                .Add(new HitActionSystem())
                .Add(new TakeDamageSystem())
                .Add(new DestroyVfxItemSystem())
                .Add(new CreateEnemySystem())
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