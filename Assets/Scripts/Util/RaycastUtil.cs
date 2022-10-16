using UnityEngine;

namespace Util
{
    public static class RaycastUtil
    {
        public static bool TryGetComponent<T>(Ray ray, out T component, float distance = 100f, 
            bool isShowDebugLine = false, LayerMask layerMask = default)  where T : class
        {
            component = null;
            
            if (isShowDebugLine)
                UnityEngine.Debug.DrawRay(ray.origin, ray.direction * distance, Color.red, 1f);

            if (Physics.Raycast(ray, out var hit, distance, layerMask))
            {
                component = hit.collider.GetComponentInParent<T>();
                return component != null;
            };

            return false;
        }


        public static bool TryGetComponent<T>(Ray ray, out T component,  LayerMask layerMask) where T : class
        {
            return TryGetComponent(ray,  out component, 100f, false, layerMask);
        }


        public static bool TryGetComponent<T>(Ray ray, out T component,  LayerMask layerMask, 
            bool isShowDebugLine) where T : class
        {
            return TryGetComponent(ray,  out component, 100f, isShowDebugLine, layerMask);
        }
    }
}