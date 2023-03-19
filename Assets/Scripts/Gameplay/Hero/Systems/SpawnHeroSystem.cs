using System;
using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.InputSystem.Users;

namespace BT
{
    public sealed class SpawnHeroSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var entities = world.Filter<CreateHeroRequest>().End();
            var requestPool = world.GetPool<CreateHeroRequest>();

            foreach(var e in entities)
            {
                ref var request = ref requestPool.Get(e);

                Spawn(world, data, ref request);
                SpawnHeroEvent(world);
                
                requestPool.Del(e);
            }
        }


        private void SpawnHeroEvent(EcsWorld world)
        {
            var entity = world.NewEntity();
            var eventPool = world.GetPool<HeroCreatedEvent>();
            eventPool.Add(entity);
        }


        private void Spawn(EcsWorld world, SharedData data, ref CreateHeroRequest request)
        {
            var heroGO = UnityEngine.Object.Instantiate
            (
                data.Config.PlayerData.Prefab, 
                data.WorldData.HeroSpawnPoints[request.HeroID].position, 
                Quaternion.identity
            );

            var entity = world.NewEntity();

            //hero
            var heroTagPool = world.GetPool<HeroTag>();
            heroTagPool.Add(entity);

            //input
            var inputDataPool =  world.GetPool<CharacterCommand>();
            inputDataPool.Add(entity);

            //movement
            var movementPool =  world.GetPool<CharacterControllerMovement>();
            ref var movement = ref movementPool.Add(entity);
            var characterController = heroGO.GetComponent<CharacterController>();
            movement.characterController = characterController;
            movement.Transform = heroGO.transform;   

            //view
            var viewPool = world.GetPool<CharacterView>();
            ref var view = ref viewPool.Add(entity);
            view.Animator = heroGO.GetComponentInChildren<Animator>();
            view.ViewTransform = heroGO.transform.GetChild(0).transform; 
            view.Height = characterController.height;
            view.BodyRadius = characterController.radius;
          
            //hit interaction
            var hitPool = world.GetPool<HitInteraction>();
            ref var hit = ref hitPool.Add(entity);
            hit.HitView = heroGO.GetComponent<IHitReceiver>();
            hit.HitBoxes = heroGO.GetComponentsInChildren<HitBox>();
            Array.ForEach(hit.HitBoxes, h => h.Init());
            

           //attack
            var heroHandleAttackPool = world.GetPool<HeroAttack>();
            ref var heroAttack = ref heroHandleAttackPool.Add(entity);            
            heroAttack.IsActiveAttack = false;    
            heroAttack.IsNeedFinishAttack = false;
            heroAttack.CurrentPunch = null; 
            heroAttack.CurrentKick = null; 
            heroAttack.KickQueue = new Queue<HeroAttackAnimationData>();
            heroAttack.PunchQueue = new Queue<HeroAttackAnimationData>();
            heroAttack.PunchData = data.Config.HeroAttackData.PunchAnimationData;
            heroAttack.KickData = data.Config.HeroAttackData.KickAnimationData;
            heroAttack.PunchFinishData = data.Config.HeroAttackData.PunchAnimationFinishData;
            heroAttack.KickFinishData = data.Config.HeroAttackData.KickAnimationFinishData;

            //HP
            var healthPool = world.GetPool<Health>();
            ref var heroHealth = ref healthPool.Add(entity); 
            heroHealth.HP = heroHealth.MaxHP = data.Config.PlayerData.StartHP;          
            heroHealth.IsChangeValue = true;

            //input
            var heroInputPool = world.GetPool<HeroInputUser>();
            ref var input = ref heroInputPool.Add(entity);
            var action = new InputServices();   
            input.InputProvider = new InputHandleProvider(action);
            input.InputProvider.Enable();
            input.InputProvider.ResetInput();
            input.Device = request.Device;
            input.User = InputUser.PerformPairingWithDevice(request.Device);

            BindDeviceToUser(action, ref input);

            Util.Debug.PrintColor($"Hero init: {input.Device.name}", UnityEngine.Color.green);
        }


        private void BindDeviceToUser(InputServices action, ref HeroInputUser inputUser)
        {
            var deviceName = inputUser.Device.name;
            inputUser.User.ActivateControlScheme(action.controlSchemes.First(s => s.name.Equals(deviceName)));
            inputUser.User.AssociateActionsWithUser(action);
        }
    }
}