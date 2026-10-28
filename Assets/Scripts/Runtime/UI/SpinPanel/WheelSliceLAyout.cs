using UnityEngine;

namespace Runtime.UI
{
    public class WheelSliceLAyout
    {
        public static void GetPose(int index, int count, float radius, float startAngle, out Vector2 position, out float rotationZ)
        {
            float angle = startAngle + index * (360f / count);
            float radian = angle * Mathf.Deg2Rad;
            position = new Vector2(Mathf.Sin(radian), Mathf.Cos(radian)) * radius;
            rotationZ = -angle;
        }
    }
}
