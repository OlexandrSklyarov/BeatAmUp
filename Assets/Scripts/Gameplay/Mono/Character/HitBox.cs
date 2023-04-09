using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace BT
{
    [RequireComponent(typeof(Rigidbody))]
    public class HitBox : MonoBehaviour, IHitReceiver
    {
        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.isKinematic = true;
        }

        private void Start() => OnValidate();
        
        
        [Conditional("UNITY_EDITOR")]
        private void OnValidate()
        {
            EditorApplication.delayCall += () =>
            {
                gameObject.layer = LayerMask.NameToLayer(ConstPrm.Character.HIT_LAYER_NAME);
            };
        }


        void IHitReceiver.AddForceDamage(Vector3 force)
        {
            _rb.AddForce(force, ForceMode.Impulse);
        }
    }
}