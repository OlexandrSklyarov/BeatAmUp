using System;
using UnityEngine;

namespace BT
{
    [Serializable]
    public class HUD
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private CharacterViewInfo _heroViewInfo;
        [SerializeField] private CharacterViewInfo _enemyViewInfo;


        public void Init()
        {
            EnableEnemyHPBar(false);
        }        


        public void Show() => _root.gameObject.SetActive(true);


        public void Hide() => _root.gameObject.SetActive(false);


        public void ChangePlayerHP(float previous, float current)
        {
            _heroViewInfo.ChangeHpBarWithDelay(previous, current, ConstPrm.UI.CHANGE_HP_BAR_DURATION);
        }


        public void ChangeEnemyHP(float previous, float current)
        {
            EnableEnemyHPBar(true);

            _enemyViewInfo.ChangeHpBarWithDelay(previous, current, ConstPrm.UI.CHANGE_HP_BAR_DURATION, () =>
            {
                EnableEnemyHPBar(false);
            });
        }


        private void EnableEnemyHPBar(bool isActive) => _enemyViewInfo.gameObject.SetActive(isActive);
    }
}