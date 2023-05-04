using Leopotam.EcsLite;

namespace BT
{
    public class PrepareDeactivateRagdollSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
                
            var enemies = world
                .Filter<RagdollState>()
                .Inc<CharacterPhysicsBody>()
                .Inc<CharacterView>()
                .Inc<MovementAI>()
                .Exc<DeactivateRagdoll>()
                .Exc<Stun>()
                .Exc<Death>()
                .End();

            var deactivateRagdollPool = world.GetPool<DeactivateRagdoll>();
            var bodyPool = world.GetPool<CharacterPhysicsBody>();
            var viewPool = world.GetPool<CharacterView>();
            var aiPool = world.GetPool<MovementAI>();

            foreach (var ent in enemies)
            {
                ref var body = ref bodyPool.Get(ent);
                ref var view = ref viewPool.Get(ent);
                ref var ai = ref aiPool.Get(ent);

                GameplayExtensions.PopulateBoneTransforms(body.Bones, body.RagdollBoneTransforms);                

                ref var comp = ref deactivateRagdollPool.Add(ent);
                comp.IsCanStandUp = IsCanStandUp(ref view, ref ai);
            }
        }


        private bool IsCanStandUp(ref CharacterView view, ref MovementAI ai)
        {            
            var origin = view.HipBone.position;
            var isStandSuccess = ai.NavAgent.Warp(origin);

            return isStandSuccess;
        }
    }
}