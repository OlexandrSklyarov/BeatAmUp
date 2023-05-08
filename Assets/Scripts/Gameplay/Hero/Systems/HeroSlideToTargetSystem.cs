using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public class HeroSlideToTargetSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var entities = world
                .Filter<Hero>()
                .Inc<HeroSlideToTarget>()
                .Inc<CharacterControllerMovement>()
                .Inc<Translation>()
                .Inc<CharacterView>()
                .End();

            var heroPool = world.GetPool<Hero>();
            var translationPool = world.GetPool<Translation>();
            var ccPool = world.GetPool<CharacterControllerMovement>();
            var viewPool = world.GetPool<CharacterView>();            
            var slidePool = world.GetPool<HeroSlideToTarget>();

            foreach (var ent in entities)
            {
                ref var hero = ref heroPool.Get(ent);
                ref var slide = ref slidePool.Get(ent);
                ref var cc = ref ccPool.Get(ent);
                ref var heroTranslation = ref translationPool.Get(ent);
                ref var heroView = ref viewPool.Get(ent);

                if (IsSlideCompleted(ref slide, ref heroView, ref cc, ref heroTranslation, ref hero))
                {
                    slidePool.Del(ent);
                }
            }
        }
        
        
        private bool IsSlideCompleted(ref HeroSlideToTarget slide, ref CharacterView heroView,
            ref CharacterControllerMovement cc, ref Translation heroTranslation, ref Hero hero)
        {
            var target = slide.TargetPosition;
            target.y = heroTranslation.Value.position.y;
            var toTarget = target - heroTranslation.Value.position;
            
            //move
            cc.CharacterController.enabled = false;
                
            heroTranslation.Value.position = Vector3.MoveTowards
            (
                heroTranslation.Value.position, 
                target,
                hero.Data.SlideSpeed * Time.deltaTime
            );
                
            cc.CharacterController.enabled = true;

            //rotate
            heroView.ViewTransform.rotation = Util.Vector3Math.DirToQuaternion(toTarget);
            
            
            return Vector3.SqrMagnitude(heroTranslation.Value.position - target) <= slide.TargetSqBodyRadius;
        }
    }
}