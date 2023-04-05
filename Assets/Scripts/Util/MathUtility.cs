using UnityEngine;

namespace Util
{
    public static class MathUtility
    {
        public static float GetBallisticsPushForce(Vector3 startPosition, Vector3 target, float angleDegrees, float distanceOffset = 0f)
        {
            var fromTo = target - startPosition;
            var fromToXZ = new Vector3(fromTo.x, 0f, fromTo.z);

            var x = fromToXZ.magnitude + distanceOffset;
            var y = fromTo.y;
                
            var angleRadians = angleDegrees * Mathf.PI / 180f;
            var g = Physics.gravity.y;

            var v2 = (g * x * x) / (2 * (y - Mathf.Tan(angleRadians) * x) * Mathf.Pow(Mathf.Cos(angleRadians), 2f));
            var v = Mathf.Sqrt(Mathf.Abs(v2));

            return v;
        }

        
        public static Vector3 GetCirclePosition2D(Vector3 origin, float maxAngle, 
            int maxCount, int currentIndex, float radius)
        {
            return GetCirclePosition2D(origin, Vector3.zero, maxAngle, maxCount, currentIndex, radius);
        }
        
        
        public static Vector3 GetCirclePosition2D(Vector3 origin, Vector3 direction, 
            float maxAngle, int maxCount, int currentIndex, float radius)
        {
            radius = Mathf.Max(0, radius);
            currentIndex = Mathf.Max(0, currentIndex);
            maxCount = Mathf.Max(0, maxCount);
            maxAngle = Mathf.Clamp(maxAngle, 0f, 360f);
            
            var arcLenght = (maxAngle / 360f) * 2f * Mathf.PI;
            var nextAngle = arcLenght / maxCount;

            var angle = Util.Vector3Math.GetUpAxisAngleRotate(direction) + nextAngle * currentIndex;
            
            var z = origin.z + Mathf.Cos(angle) * radius;
            var x = origin.x + Mathf.Sin(angle) * radius;
            return  new Vector3(x, origin.y, z);
        }
    }
}