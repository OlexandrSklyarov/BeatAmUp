using UnityEngine;

namespace BT
{
    public class GameWorldVFX : BaseVFXItem
    {
        public VfxType Type => _type;

        [SerializeField] private VfxType _type;
    }
}