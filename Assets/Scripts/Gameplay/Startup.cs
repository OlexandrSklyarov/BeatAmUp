using System;
using System.Threading.Tasks;
using Gameplay.FX;
using Leopotam.EcsLite;
using Services.Scenes.LoadingScreen;
using UnityEditor.SearchService;
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


        private async void Start()
        {   
            await WaitLoadScene();

            _world = new EcsWorld();

            var inputService = new InputServices();

            var data = new SharedData()
            {
                Config = _gameConfig,
                WorldData = _worldData,
                InputProvider = new InputHandleProvider(inputService),
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


        private async Task WaitLoadScene()
        {
            while(LoadingScreenView.Instance != null) await Task.Delay(100);
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
                .Add(new InitPlayerControlSystem())
                .Init();
        }


        private void AddSystems()
        {
            _updateSystems  

                //editor debug
            #if UNITY_EDITOR
                .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
            #endif
                //gravity
                .Add(new CharacterControllerCheckGroundSystem())
                .Add(new CharacterControllerApplyGravitySystem())

                //hero
                .Add(new SpawnHeroSystem())
                .Add(new PlayerInputSystem())                
                .Add(new HeroFindNearEnemyTargetSystem())
                .Add(new HeroSlideToTargetSystem())
                .Add(new HeroTryExecutePowerDamageSystem())
                .Add(new HeroJumpSystem())
                .Add(new HeroRotateViewSystem())
                .Add(new HeroChangeHorizontalVelocitySystem())
                .Add(new HeroSlowDownHorizontalVelocitySystem())
                .Add(new ApplyHorizontalVelocitySystem())
                .Add(new HeroDieSystem())

                //combat
                .Add(new TryResetCombatCommandSystem())
                .Add(new CharacterComboAttackSystem())
                .Add(new HeroRotateToNearTargetSystem())

                //character hit
                .Add(new ResetHitEventSystem())
                .Add(new CharacterHitSystem())
                .Add(new CharacterApplyDamageSystem())
                .Add(new CharacterDetectDeathSystem())
                .Add(new CharacterActiveStunSystem())
                .Add(new CharacterActiveRagdollSystem())
                .Add(new CharacterRestoreStunSystem())
                .Add(new PrepareRestoreRagdollSystem())
                .Add(new EnemyCharacterRestoreRagdollSystem())
                .Add(new ResetHitCountSystem())
                .Add(new CharacterAddBlockMovementSystem())
                .Add(new CharacterUnBlockMovementSystem())

                //enemy
                .Add(new CheckEnemySpawnSystem())
                .Add(new SpawnEnemySystem())
                .Add(new EnemyCheckAliveTargetHeroSystem())
                .Add(new EnemyFindTargetHeroSystem())
                .Add(new EnemyAssigningPointNearTargetSystem())
                .Add(new EnemyTryAddAttackStateSystem())
                .Add(new EnemyAttackTargetSystem())
                .Add(new EnemyApplyNavMeshDestinationSystem())
                .Add(new EnemyBodyRotateSystem())
                .Add(new EnemyCharacterDieSystem())

                //Animation
                .Add(new HeroAnimationSystem())
                .Add(new CharacterDamageAnimationSystem())
                .Add(new CharacterAttackAnimationSystem())
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