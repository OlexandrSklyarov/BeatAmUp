using System;
using UnityEngine;
using UnityEngine.UI;

namespace BT
{
    public class CharacterViewInfo : MonoBehaviour
    {
        [SerializeField] private RawImage _icon;
        [SerializeField] private Slider hpBar;


        public void ChangeIcon(Texture texture) => _icon.texture = texture;


        public void ChangeHpBarWithDelay(float previous, float current, float duration, Action onCompleted = null)
        {
            LeanTween.value(previous, current, duration)
                .setOnUpdate((v) => hpBar.value = v)
                .setEase(LeanTweenType.easeOutSine)
                .setOnComplete(() => onCompleted?.Invoke());
        }
    }
}