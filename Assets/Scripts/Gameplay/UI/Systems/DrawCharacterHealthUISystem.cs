using Leopotam.EcsLite;

namespace BT
{
    public sealed class DrawCharacterHealthUISystem : IEcsInitSystem, IEcsRunSystem
    {
        public void Init(IEcsSystems systems)
        {
            var data = systems.GetShared<SharedData>();
            data.WorldData.GamaUI.Init(data.Config.UI);
        }


        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var ui = systems.GetShared<SharedData>().WorldData.GamaUI;

            var heroEntities = world
                .Filter<Hero>()
                .Inc<Health>()
                .Inc<ChangeHealthFlag>()
                .End();

            var enemyEntities = world
                .Filter<Enemy>()
                .Inc<Health>()
                .Inc<ChangeHealthFlag>()
                .End();

            var healthPool = world.GetPool<Health>();
            var healthFlagPool = world.GetPool<ChangeHealthFlag>();
            var heroPool = world.GetPool<Hero>();

            foreach(var h in heroEntities)
            {
                ref var heroHealth = ref healthPool.Get(h);
                ref var hero = ref heroPool.Get(h);
                TryChangeHealthUI(ref heroHealth, CharacterType.HERO, ui, hero.ID);
                
                healthFlagPool.Del(h);
            }

            foreach(var e in enemyEntities)
            {
                ref var enemyHealth = ref healthPool.Get(e);
                TryChangeHealthUI(ref enemyHealth, CharacterType.ENEMY, ui);
                
                healthFlagPool.Del(e);
            }
        }


        private void TryChangeHealthUI(ref Health hp, CharacterType type, GameUI ui, int id = -1)
        {
            ui.ChangeCharacterHP
            (
                (float)hp.PreviousHP / hp.MaxHP,
                (float)hp.CurrentHP / hp.MaxHP,
                type,
                id
            );
        }
    }
}