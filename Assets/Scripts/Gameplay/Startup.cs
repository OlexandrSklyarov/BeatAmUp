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
                VFXController = new VisualFXController(_gameConfig.VfxConfig),
                EnemyFactory = new EnemyFactory(_gameConfig.EnemyConfig.EnemyPoolData)
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
                .Add(new InitCameraSystem())
                .Add(new InitSpawnEnemyZoneSystem())
                .Add(new InitTestPunchBoxSystem())
                .Add(new InitPlayerControlSystem())
                .Init();
        }


        private void AddSystems()
        {
            _updateSystems  
                //hero
                .Add(new SpawnHeroSystem())
                .Add(new PlayerInputSystem())
                .Add(new CharacterControllerCheckGroundSystem())
                .Add(new HeroSlideToNearestEnemySystem())
                .Add(new CharacterControllerApplyGravitySystem())
                .Add(new HeroSittingSystem())
                .Add(new HeroComboAttackSystem())
                .Add(new HeroJumpSystem())
                .Add(new HeroRotateViewSystem())
                .Add(new HeroChangeHorizontalVelocitySystem())
                .Add(new HeroSlowDownHorizontalVelocitySystem())
                .Add(new ApplyHorizontalVelocitySystem())
                .Add(new HeroAnimationSystem())

                //character hit
                .Add(new CharacterHitHandlerSystem())
                .Add(new ResetHitCountSystem())
                .Add(new CharacterTakeDamageSystem())
                .Add(new CharacterReleaseStunSystem())

                //enemy
                .Add(new CreateEnemySystem())
                .Add(new EnemyFindTargetHeroSystem())
                .Add(new EnemySetTargetDestinationSystem())
                .Add(new EnemyBodyRotateSystem())
                .Add(new EnemyDamageHandlerSystem())
                .Add(new EnemyCharacterDieSystem())
                .Add(new EnemyAnimationSystem())

                //UI
                .Add(new DrawCharacterUISystem())

                //Camera
                .Add(new CameraUpdateTargetSystem())
                .Add(new ShakeCameraSystem())

                //vfx
                .Add(new DestroyVfxItemSystem())
                .Init();
            
            _fixedUpdateSystems
                .Init();

            _lateUpdateSystems
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