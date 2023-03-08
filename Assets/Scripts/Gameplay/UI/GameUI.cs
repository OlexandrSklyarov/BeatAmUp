using UnityEngine;

namespace BT
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private HUD _hud;

        public void Init()
        {
            _hud.Init();
        }


        public void ChangeCharacterHP(float previous, float current, CharacterType type)
        {
            switch(type)
            {
                case CharacterType.HERO:
                    _hud.ChangePlayerHP(previous, current);
                break;

                case CharacterType.ENEMY:
                    _hud.ChangeEnemyHP(previous, current);
                break;
            }
        }        
    }
}