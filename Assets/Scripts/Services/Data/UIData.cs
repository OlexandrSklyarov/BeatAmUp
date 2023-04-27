using System;
using UnityEngine;

namespace BT
{
    [CreateAssetMenu(fileName = "UIConfig",menuName = "SO/UIConfig")]
    public class UIData : ScriptableObject
    {
        [field: SerializeField] public HUDConfig HUD { get; private set; }
    }
    
    
    [Serializable]
    public sealed class HUDConfig
    {
        [field: SerializeField] public CharacterHudItem HudHudItemPrefab { get; private set; }
        [field: SerializeField] public CharacterIcon[] Icons { get; private set; }
    }


    [Serializable]
    public sealed class CharacterIcon
    {
        [field: SerializeField] public CharacterType Type { get; private set; }
        [field: SerializeField] public Texture2D Texture { get; private set; }
    }
}