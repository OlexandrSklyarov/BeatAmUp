using System;
using Cinemachine;
using UnityEngine;

namespace Services.Data
{
    [Serializable]
    public class WorldData
    {
        public Transform HeroSpawnPoint;
        public CinemachineVirtualCamera GameVC;

    }
}