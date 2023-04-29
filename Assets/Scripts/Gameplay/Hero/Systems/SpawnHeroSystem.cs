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

            ClearCreateNewHeroEvents(world);

            var entities = world.Filter<CreateHeroRequest>().End();
            var requestPool = world.GetPool<CreateHeroRequest>();

            foreach(var e in entities)
            {
                ref var request = ref requestPool.Get(e);

                Spawn(world, data, ref request);
                SpawnHeroEvent(world, ref request);
                
                requestPool.Del(e);
            }
        }

        
        private void ClearCreateNewHeroEvents(EcsWorld world)
        {
            var filter = world.Filter<CreateNewHeroEvent>().End();
            var eventPool = world.GetPool<CreateNewHeroEvent>();

            foreach (var ent in filter)
            {
                eventPool.Del(ent);
            }
        }


        private void SpawnHeroEvent(EcsWorld world, ref CreateHeroRequest spawnRequest)
        {
            var entity = world.NewEntity();
            var eventPool = world.GetPool<CreateNewHeroEvent>();
            ref var evt = ref eventPool.Add(entity);
            evt.NewHeroID = spawnRequest.HeroID;
        }


        private void Spawn(EcsWorld world, SharedData data, ref CreateHeroRequest spawnRequest)
        {
            var id = spawnRequest.HeroID;
            var unitData = data.Config.Heroes.First(u => u.ID == id);
            
            var heroView = UnityEngine.Object.Instantiate
            (
                unitData.Prefab, 
                data.WorldData.HeroSpawnPoints[spawnRequest.HeroID].position, 
                Quaternion.identity
            );

            var entity = world.NewEntity();

            //hero
            ref var hero = ref world.GetPool<Hero>().Add(entity);
            hero.ID = spawnRequest.HeroID;
            hero.Data = unitData.Data;

            //input
            world.GetPool<CharacterCommand>().Add(entity);
            

            //movement
            var movementPool =  world.GetPool<CharacterControllerMovement>();
            ref var movement = ref movementPool.Add(entity);
            var characterController = heroView.GetComponent<CharacterController>();
            movement.CharacterController = characterController;

            //translation
            var translationPool =  world.GetPool<Translation>();
            ref var translation = ref translationPool.Add(entity);
            translation.Value = heroView.transform;   


            //view
            var viewPool = world.GetPool<CharacterView>();
            ref var view = ref viewPool.Add(entity);
            view.Animator = heroView.GetComponentInChildren<Animator>();
            view.ViewTransform = heroView.transform.GetChild(0).transform; 
            view.Height = characterController.height;
            view.BodyRadius = characterController.radius;
            view.HipBone = heroView.BodyHips;
          

            //hit interaction
            var hitPool = world.GetPool<HitInteraction>();
            ref var hit = ref hitPool.Add(entity);
            hit.HitBoxes = heroView.GetComponentsInChildren<HitBox>();
            hit.HurtBoxes = heroView.GetComponentsInChildren<HurtBox>();
            foreach(var h in hit.HurtBoxes) h.Init();
            

           //attack
            var heroHandleAttackPool = world.GetPool<CharacterAttack>();
            ref var heroAttack = ref heroHandleAttackPool.Add(entity);            
            heroAttack.IsActiveAttack = false;    
            heroAttack.IsNeedFinishAttack = false;
            heroAttack.CurrentPunch = null; 
            heroAttack.CurrentKick = null; 
            heroAttack.KickQueue = new Queue<HeroAttackAnimationData>();
            heroAttack.PunchQueue = new Queue<HeroAttackAnimationData>();


            //HP
            var healthPool = world.GetPool<Health>();
            ref var heroHealth = ref healthPool.Add(entity); 
            heroHealth.CurrentHP = heroHealth.MaxHP = unitData.Data.StartHP;


            //input
            var heroInputPool = world.GetPool<HeroInputUser>();
            ref var input = ref heroInputPool.Add(entity);
            var action = new InputServices();   
            input.InputProvider = new InputHandleProvider(action);
            input.InputProvider.Enable();
            input.Device = spawnRequest.Device;
            input.User = InputUser.PerformPairingWithDevice(spawnRequest.Device);

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