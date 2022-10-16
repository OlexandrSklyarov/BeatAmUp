using System;
using Gameplay.Environment;
using Gameplay.Test;
using Leopotam.EcsLite;
using Services.Data;
using Unity.VisualScripting;
using UnityEngine;

namespace Gameplay
{
    public sealed class Startup : MonoBehaviour
    {        
        private EcsWorld _world;
        private IEcsSystems _initSystems;
        private IEcsSystems _updateSystems;


        private void Start()
        {
            _world = new EcsWorld();

            var data = new SharedData();
            
            _initSystems = new EcsSystems(_world, data);
            _updateSystems = new EcsSystems(_world, data);

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
        }


        private void Update() => _updateSystems?.Run();


        private void OnDestroy()
        {
            _initSystems?.Destroy();
            _initSystems = null;

            _updateSystems?.Destroy();
            _updateSystems = null;
        }
    }
}