using System;
using UnityEngine;

namespace BT
{
    [Serializable]
    public class GameRules
    {
        [field: SerializeField, Range(1, 4)] public int MaxPlayerCount { get; private set; } = 2;
        [field: SerializeField] public ControlDeviceType ControlType { get; private set; } = ControlDeviceType.KEYBOARD;

    }
}