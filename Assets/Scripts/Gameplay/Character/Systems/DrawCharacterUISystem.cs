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
                ref var heroHP = ref hpPool.Get(h);
                
                if (heroHP.IsChangeValue)
                {
                    ui.ChangeCharacterHP
                    (
                        (float)heroHP.PreviousHP / heroHP.MaxHP, 
                        (float)heroHP.HP / heroHP.MaxHP,
                        CharacterType.HERO
                    );
                }

                heroHP.IsChangeValue = false;
            }

            foreach(var e in enemyEntities)
            {
                ref var enemyHP = ref hpPool.Get(e);
                
                if (enemyHP.IsChangeValue)
                {
                    ui.ChangeCharacterHP
                    (
                        (float)enemyHP.PreviousHP / enemyHP.MaxHP, 
                        (float)enemyHP.HP / enemyHP.MaxHP,
                        CharacterType.ENEMY
                    );
                }

                enemyHP.IsChangeValue = false;
            }
        }
    }
}