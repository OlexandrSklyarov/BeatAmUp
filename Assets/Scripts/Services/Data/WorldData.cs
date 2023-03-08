using System;
using Cinemachine;
using UnityEngine;

namespace BT
{
    [Serializable]
    public class WorldData
    {
        public Transform HeroSpawnPoint;
        public CinemachineVirtualCamera GameVirtualCamera;
        public GameUI GamaUI;
    }
}