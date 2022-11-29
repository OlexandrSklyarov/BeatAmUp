using Gameplay.Mono.Character;
using Gameplay.Mono.Hero;
using Leopotam.EcsLite;
using Services.Data;
using UnityEngine;

namespace Gameplay.Character.Hero
{
    public sealed class InitHeroSystem : IEcsInitSystem
    {
        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var heroGO = Object.Instantiate
            (
                data.Config.PlayerData.Prefab, 
                data.WorldData.HeroSpawnPoint.position, 
                Quaternion.identity
            );

            var heroEntity = world.NewEntity();
            
            var heroTagPool =  world.GetPool<HeroTag>();
            heroTagPool.Add(heroEntity);

            var inputDataPool =  world.GetPool<PlayerInputData>();
            inputDataPool.Add(heroEntity);

            var viewPool = world.GetPool<CharacterView>();
            ref var view = ref viewPool.Add(heroEntity);
            view.Animator = heroGO.GetComponentInChildren<Animator>();
            view.ViewTransform = heroGO.transform.GetChild(0).transform;             

            var movementPool =  world.GetPool<Movement>();
            ref var movement = ref movementPool.Add(heroEntity);
            movement.Body = heroGO.GetComponent<Rigidbody>();
            movement.Transform = heroGO.transform; 

            var colView = movement.Transform.GetComponent<CharacterCollisionView>(); 
            colView.Init(world, heroEntity, data.Config.CharacterData.GroundLayer);

            var heroHandleAttackPool = world.GetPool<HeroAttack>();
            ref var heroAttack = ref heroHandleAttackPool.Add(heroEntity);            
            heroAttack.ComboTimer = ConstPrm.Hero.DEFAULT_COMBO_TIMER;    
            heroAttack.CurrentPunchState = PunchState.NONE; 
            heroAttack.CurrentKickState = KickState.NONE; 
            
            Util.Debug.Print($"hero init...");
        }
    }
}