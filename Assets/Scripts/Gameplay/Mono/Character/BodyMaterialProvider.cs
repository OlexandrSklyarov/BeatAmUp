using System;
using System.Linq;
using UnityEngine;

namespace BT
{
    [Serializable]
    public class BodyMaterialProvider
    {
        private readonly GameObject _parentGO;
        private readonly Material[] _bodyMaterials;
        private static int _dissolveID = Shader.PropertyToID("_Dissolve");


        public BodyMaterialProvider(SkinnedMeshRenderer[] renderers)
        {
            _parentGO = renderers[0].transform.parent.gameObject;
            _bodyMaterials = renderers.Select(r => r.material).ToArray();

            ChangeDissolveValue(1f);
        }


        public void SetDissolveValueSmooth(float value, float changeDuration = 3f)
        {
            value = Mathf.Clamp01(value);
            var curValue = _bodyMaterials[0].GetFloat(_dissolveID);

            LeanTween
                .value(_parentGO, curValue, value, changeDuration)
                .setOnUpdate((v) => ChangeDissolveValue(v));            
        }


        public void ChangeDissolveValue(float v)
        {
            Array.ForEach(_bodyMaterials, m => m.SetFloat(_dissolveID, v));
        }
    }
}