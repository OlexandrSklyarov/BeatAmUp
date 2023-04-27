using System;
using UnityEngine;
using UnityEngine.UI;

namespace BT
{
    public class CharacterHudItem : MonoBehaviour
    {
        [SerializeField] private RawImage _icon;
        [SerializeField] private GameObject _iconRoot;
        [SerializeField] private Slider _hpBar;


        public void ChangeIcon(Texture texture) => _icon.texture = texture;


        public void ChangeHpBarWithDelay(float previous, float current, float duration, Action onCompleted = null)
        {
            LeanTween.value(previous, current, duration)
                .setOnUpdate((v) => _hpBar.value = v)
                .setEase(LeanTweenType.easeOutSine)
                .setOnComplete(() => onCompleted?.Invoke());
        }


        public void SetActive(bool isActive)
        {
            _iconRoot.gameObject.SetActive(isActive);
            _hpBar.gameObject.SetActive(isActive);
        }
    }
}