using System.Linq;
using Leopotam.EcsLite;

namespace BT
{
    public sealed class InitPlayerControlSystem : IEcsInitSystem
    {
        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();
            
            InitPlayerControl(world, data); 
        }

        private void InitPlayerControl(EcsWorld world, SharedData data)
        {
            if (!data.PlayerInputService.IsHasFreeController) return;
            if (!data.PlayerInputService.IsHasFreeControllers(data.GameSettings.PlayerCount)) return;

            for (int i = 0; i < data.GameSettings.PlayerCount; i++)
            {
                CreateHeroRequest(world, data, i);
            }
        }

        private void CreateHeroRequest(EcsWorld world, SharedData data, int heroIndex)
        {     
            var requestPool = world.GetPool<CreateHeroRequest>(); 

            var requestEntity = world.NewEntity();                       
            ref var request = ref requestPool.Add(requestEntity);            
            request.SpawnIndex = heroIndex;
            request.UnitData = GetHeroUnitData(data, heroIndex);
        }


        private HeroUnit GetHeroUnitData(SharedData data, int index)
        {
            return (data.Config.Heroes.Length <= index + 1) ? 
                data.Config.Heroes[index] :
                data.Config.Heroes.Last();
        }
    }
}