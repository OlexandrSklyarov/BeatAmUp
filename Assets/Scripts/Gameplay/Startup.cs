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
                EnemyFactory = new EnemyFactory(_gameConfig.EnemyConfig.EnemyPoolData),
                CollisionService = new CheckCollisionServices()
            };
            
            _initSystems = new EcsSystems(_world, data);
            _updateSystems = new EcsSystems(_world, data);

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
                .Add(new HeroFindNearEnemyTargetSystem())
                .Add(new HeroSlideToTargetSystem())
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
                .Add(new CharacterTryHitSystem())
                .Add(new CharacterApplyDamageSystem())
                .Add(new CharacterActiveStunSystem())
                .Add(new CharacterActiveRagdollSystem())
                .Add(new CharacterReleaseStunSystem())
                .Add(new EnemyCharacterDeactivateRagdollSystem())
                .Add(new ResetHitCountSystem())
                .Add(new CharacterDamageAnimationSystem())

                //enemy
                .Add(new SpawnEnemySystem())
                .Add(new EnemyFindTargetHeroSystem())
                .Add(new EnemySetTargetDestinationSystem())
                .Add(new EnemyBodyRotateSystem())
                .Add(new EnemyCharacterDieSystem())
                .Add(new EnemyAnimationSystem())

                //UI
                .Add(new DrawCharacterHealthUISystem())
                .Add(new AddNewCharacterHudSystem())

                //Camera
                .Add(new CameraUpdateTargetSystem())
                .Add(new CreateShakeCameraEventSystem())
                .Add(new ShakeCameraHandlerSystem())

                //vfx
                .Add(new CreateDamageVfxItemSystem())
                .Add(new CreateDeathVfxItemSystem())
                .Add(new DestroyCompletedVfxItemSystem())
                .Init();   
        }


        private void Update() => _updateSystems?.Run();          


        private void OnDestroy()
        {
            _initSystems?.Destroy();
            _initSystems = null;

            _updateSystems?.Destroy();
            _updateSystems = null;            

            _world?.Destroy();
            _world = null;
        }
    }
}