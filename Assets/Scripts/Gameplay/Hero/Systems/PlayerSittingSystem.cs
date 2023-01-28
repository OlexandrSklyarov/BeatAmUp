using Leopotam.EcsLite;

namespace BT
{
    public class PlayerSittingSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world  = systems.GetWorld();

            var entities = world
                .Filter<HeroTag>()
                .Inc<CharacterCommand>()
                .End();

            var CharacterCommand = world.GetPool<CharacterCommand>();
            var sitingPool = world.GetPool<CharacterSitDown>();
            var groundedPool = world.GetPool<CharacterGrounded>();

            foreach(var e in entities)
            {
                ref var command = ref CharacterCommand.Get(e);                

                var isSitting = sitingPool.Has(e);
                var isGrounded = groundedPool.Has(e);

                if (!isGrounded)
                {
                    if (isSitting) sitingPool.Del(e); 
                                       
                    continue;
                }

                if (command.IsSitting)
                {
                    if (!isSitting) 
                    {
                        sitingPool.Add(e);                       
                    }
                }   
                else
                {
                    if (isSitting) 
                    {
                        sitingPool.Del(e); 
                    }
                }  
            }
        }
    }
}