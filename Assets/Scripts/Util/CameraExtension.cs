using UnityEngine;

namespace Util
{
    public static class CameraExtension
    {
        public static float CalculateTargetFOV(float targetFOV, float cameraAspect)
        {
            return 2f * Mathf.Atan(Mathf.Tan(targetFOV * Mathf.Deg2Rad * 0.5f) / cameraAspect) * Mathf.Rad2Deg;
        }


        public static float CalculateOrthographicSize(float targetFOV, float desiredDistance)
        {
            return desiredDistance * Mathf.Tan(targetFOV * 0.5f * Mathf.Deg2Rad);
        }
    }
}