using System;
using Cinemachine;
using UnityEngine;

namespace BT
{
    [Serializable]
    public class WorldData
    {
        public Transform[] HeroSpawnPoints;
        public CinemachineVirtualCamera GameVirtualCamera;
        public CinemachineTargetGroup TargetGroup;
        public GameUI GamaUI;
    }
}