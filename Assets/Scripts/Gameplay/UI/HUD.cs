using System;
using UnityEngine;
using UnityEngine.UI;

namespace BT
{
    [Serializable]
    public class HUD
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private Slider _heroHpBar;
        [SerializeField] private Slider _enemyHpBar;


        public void Init()
        {
            EnableEnemyHPBar(false);
        }        


        public void Show() => _root.gameObject.SetActive(true);


        public void Hide() => _root.gameObject.SetActive(false);


        public void ChangePlayerHP(float previous, float current)
        {
            LeanTween.value(previous, current, ConstPrm.UI.CHANGE_HP_BAR_DURATION)
                .setOnUpdate((v) => _heroHpBar.value = v);
        }


        public void ChangeEnemyHP(float previous, float current)
        {
            EnableEnemyHPBar(true);

            LeanTween.value(previous, current, ConstPrm.UI.CHANGE_HP_BAR_DURATION)
                .setOnUpdate((v) => _enemyHpBar.value = v)
                .setOnComplete(() => EnableEnemyHPBar(false));
        }


        private void EnableEnemyHPBar(bool isActive) => _enemyHpBar.gameObject.SetActive(isActive);
    }
}