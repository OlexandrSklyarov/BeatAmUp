using UnityEngine;

namespace BT
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private HUD _hud;
        
        private UIData _config;

        public void Init(UIData config)
        {
            _config = config;
            _hud.Init(config.HUD);
        }
        
        
        public void AddCharacterView(int id, CharacterType type)
        {
            _hud.AddCharacter(id, type);
            _hud.ChangeEnemyHP(100f, 100f);
        }
        

        public void ChangeCharacterHP(float previous, float current, CharacterType type, int id = 0)
        {
            switch(type)
            {
                case CharacterType.HERO:
                    _hud.ChangePlayerHP(previous, current, id);
                break;

                case CharacterType.ENEMY:
                    _hud.ChangeEnemyHP(previous, current);
                break;
            }
        }        
    }
}