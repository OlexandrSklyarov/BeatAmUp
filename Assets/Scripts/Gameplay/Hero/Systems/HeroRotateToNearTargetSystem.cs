using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class HeroRotateToNearTargetSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var heroEntities = world
                .Filter<TryHitEvent>()
                .Inc<Translation>()
                .Inc<CharacterView>()
                .Exc<CharacterSitDown>()
                .Exc<Stun>()
                .Exc<Death>()
                .End();

            var translationPool = world.GetPool<Translation>();  
            var viewPool = world.GetPool<CharacterView>();  

            foreach (var ent in heroEntities)
            {
                ref var tr = ref translationPool.Get(ent);  
                ref var view = ref viewPool.Get(ent);   

                if (!IsFindNearTarget(ref tr, ref view, data, out Transform target)) continue;

                RotateToTarget(ref tr, ref view, target);
            }
        }


        private bool IsFindNearTarget(ref Translation tr, ref CharacterView view, SharedData data, out Transform target)
        {
            target = default;

            var findCount = data.CollisionService.OverlapSphere
            (
                tr.Value, 
                view.BodyRadius + ConstPrm.Hero.TARGET_RADIUS_OFFSET, 
                data.Config.CharacterData.UnitLayerMask
            );

            if (findCount <= 0) return false;

            for(int i = 0; i < findCount; i++)
            {
                var hit = data.CollisionService.HitResult[i];
                
                if (hit.transform == tr.Value) continue;

                if(hit != null && IsVisible(ref tr, ref view, hit.transform.position))
                {
                    target = hit.transform;
                    return true;
                }
            }

            return false;                
        }


        private bool IsVisible(ref Translation tr, ref CharacterView view, Vector3 target)
        {
            if (Mathf.Abs(tr.Value.position.y - target.y) > view.Height) return false;

            var toTarget = target - tr.Value.position;
            
            return Vector3.Angle(toTarget, view.ViewTransform.forward) < ConstPrm.Hero.VIEW_ENEMY_ANGLE;
        }
               

        private void RotateToTarget(ref Translation tr, ref CharacterView view, Transform target)
        {
            view.ViewTransform.rotation = Util.Vector3Math.DirToQuaternion(target.position - tr.Value.position); 
        }
    }
}