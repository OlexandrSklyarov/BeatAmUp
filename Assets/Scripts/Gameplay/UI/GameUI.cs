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


        public void ChangePlayerHP(float previous, float current)
        {
            _hud.ChangePlayerHP(previous, current);
        }


        public void ChangeEnemyHP(float previous, float current)
        {
            _hud.ChangeEnemyHP(previous, current);
        }
    }
}