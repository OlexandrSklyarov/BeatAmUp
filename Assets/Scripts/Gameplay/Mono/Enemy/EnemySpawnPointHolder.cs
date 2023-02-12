using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class EnemySpawnPointHolder : MonoBehaviour
    {
        public IEnumerable<Transform> GetSpawnPoints()
        {
            foreach(Transform tr in transform)
            {
                yield return tr;
            }
        }
    }
}