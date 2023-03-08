using System;
using Cinemachine;
using UnityEngine;

namespace BT
{
    [Serializable]
    public class WorldData
    {
        public Transform HeroSpawnPoint;
        public CinemachineVirtualCamera GameVC;
        public GameUI GamaUI;
    }
}