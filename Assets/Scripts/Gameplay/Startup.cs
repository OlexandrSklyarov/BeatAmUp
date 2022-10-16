using Gameplay.Environment;
using Gameplay.Test;
using Leopotam.EcsLite;
using Services.Data;
using UnityEngine;

namespace Gameplay
{
    public sealed class Startup : MonoBehaviour
    {        
        private EcsWorld _world;
        private IEcsSystems _initSystems;
        private IEcsSystems _updateSystems;
        private IEcsSystems _fixedUpdateSystems;


        private void Start()
        {
            _world = new EcsWorld();

            var data = new SharedData();
            
            _initSystems = new EcsSystems(_world, data);
            _updateSystems = new EcsSystems(_world, data);
            _fixedUpdateSystems = new EcsSystems(_world, data);

            AddInitSystems();
            AddSystems();       
        }


        private void AddInitSystems()
        {
            _initSystems
                .Add(new InitWorldSystem())
                .Init();
        }


        private void AddSystems()
        {
            _updateSystems
                .Add(new TestRunSystem())
                .Init();
            
            _fixedUpdateSystems
                .Add(new TestFixedUpdateSystem())
                .Init();
        }


        private void Update() => _updateSystems?.Run();


        private void FixedUpdate() => _fixedUpdateSystems?.Run();


        private void OnDestroy()
        {
            _initSystems?.Destroy();
            _initSystems = null;

            _updateSystems?.Destroy();
            _updateSystems = null;

            _fixedUpdateSystems?.Destroy();
            _fixedUpdateSystems = null;

            _world?.Destroy();
            _world = null;
        }
    }
}