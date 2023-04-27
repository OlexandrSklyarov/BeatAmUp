using Leopotam.EcsLite;

namespace BT
{
    public class AddNewCharacterHudSystem : IEcsInitSystem, IEcsRunSystem
    {
        public void Init(IEcsSystems systems)
        {
            var data = systems.GetShared<SharedData>();
            data.WorldData.GamaUI.AddCharacterView(ConstPrm.UI.ENEMY_UI_ID, CharacterType.ENEMY);
        }
        
        
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var filter = world.Filter<CreateNewHeroEvent>().End();
            var heroes = world.Filter<Hero>().End();
            
            var evtPool = world.GetPool<CreateNewHeroEvent>();
            var heroPool = world.GetPool<Hero>();
            var healthFlagPool = world.GetPool<ChangeHealthflag>();

            foreach (var ent in filter)
            {
                ref var evt = ref evtPool.Get(ent);
                
                data.WorldData.GamaUI.AddCharacterView(evt.NewHeroID, CharacterType.HERO);

                foreach (var heroEnt in heroes)
                {
                    ref var hero = ref heroPool.Get(heroEnt);

                    if (hero.ID != evt.NewHeroID) continue;
                    if (healthFlagPool.Has(heroEnt)) continue;

                    healthFlagPool.Add(heroEnt);
                }
            }
        }

    }
}