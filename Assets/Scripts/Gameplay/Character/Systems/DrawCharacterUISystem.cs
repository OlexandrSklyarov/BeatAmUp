using Leopotam.EcsLite;

namespace BT
{
    public sealed class DrawCharacterUISystem : IEcsInitSystem, IEcsRunSystem
    {
        public void Init(IEcsSystems systems)
        {
            systems.GetShared<SharedData>().WorldData.GamaUI.Init();
        }


        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var ui = systems.GetShared<SharedData>().WorldData.GamaUI;

            var heroEntities = world
                .Filter<HeroTag>()
                .Inc<Health>()
                .End();

            var enemyEntities = world
                .Filter<Enemy>()
                .Inc<Health>()
                .End();

            var hpPool = world.GetPool<Health>();

            foreach(var h in heroEntities)
            {
                ref var heroHealth = ref hpPool.Get(h);
                TryChangeHealthUI(ref heroHealth, CharacterType.HERO, ui);
            }

            foreach(var e in enemyEntities)
            {
                ref var enemyHealth = ref hpPool.Get(e);
                TryChangeHealthUI(ref enemyHealth, CharacterType.ENEMY, ui);
            }
        }


        private void TryChangeHealthUI(ref Health hp, CharacterType type, GameUI ui)
        {
            if (hp.IsChangeValue)
            {
                ui.ChangeCharacterHP
                (
                    (float)hp.PreviousHP / hp.MaxHP, 
                    (float)hp.CurrentHP / hp.MaxHP,
                    type
                );
            }

            hp.IsChangeValue = false;
        }
    }
}