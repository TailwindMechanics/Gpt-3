using UnityEngine;


namespace Tailwind.Utilities.External
{
    public static class Vector3Extensions
    {
        public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 2)
        {
            var multiplier = 1f;
            for (var i = 0; i < decimalPlaces; i++)
            {
                multiplier *= 10f;
            }
            return new Vector3
            (
                Mathf.Round(vector3.x * multiplier) / multiplier,
                Mathf.Round(vector3.y * multiplier) / multiplier,
                Mathf.Round(vector3.z * multiplier) / multiplier
            );
        }

        public static Vector3 Add (this Vector3 target, Axis axis, float amount1, float? amount2 = 0f, float? amount3 = 0f)
        {
            var result = target;
            switch (axis)
            {
                case Axis.X:
                    result.x += amount1;
                    break;
                case Axis.Y:
                    result.y += amount1;
                    break;
                case Axis.Z:
                    result.z += amount1;
                    break;
                case Axis.XY:
                    result.x += amount1;
                    result.y += amount2 ?? amount1;
                    break;
                case Axis.XZ:
                    result.x += amount1;
                    result.z += amount2 ?? amount1;
                    break;
                case Axis.YZ:
                    result.y += amount1;
                    result.z += amount2 ?? amount1;
                    break;
                case Axis.XYZ:
                    result.x += amount1;
                    result.y += amount2 ?? amount1;
                    result.z += amount3 ?? amount1;
                    break;
            }

            return result;
        }

        public static Vector3 Add (this Vector3 target, Axis axis, Vector3 amount)
        {
            var result = target;
            switch (axis)
            {
                case Axis.X:
                    result.x += amount.x;
                    break;
                case Axis.Y:
                    result.y += amount.y;
                    break;
                case Axis.Z:
                    result.z += amount.z;
                    break;
                case Axis.XY:
                    result.x += amount.x;
                    result.y += amount.y;
                    break;
                case Axis.XZ:
                    result.x += amount.x;
                    result.z += amount.z;
                    break;
                case Axis.YZ:
                    result.y += amount.y;
                    result.z += amount.z;
                    break;
                case Axis.XYZ:
                    result.x += amount.x;
                    result.y += amount.y;
                    result.z += amount.z;
                    break;
            }

            return result;
        }

        public static Vector3 Multiply (this Vector3 target, Axis axis, float amount1, float? amount2 = null, float? amount3 = null)
        {
            var result = target;
            switch (axis)
            {
                case Axis.X:
                    result.x *= amount1;
                    break;
                case Axis.Y:
                    result.y *= amount1;
                    break;
                case Axis.Z:
                    result.z *= amount1;
                    break;
                case Axis.XY:
                    result.x *= amount1;
                    result.y *= amount2 ?? amount1;
                    break;
                case Axis.XZ:
                    result.x *= amount1;
                    result.z *= amount2 ?? amount1;
                    break;
                case Axis.YZ:
                    result.y *= amount1;
                    result.z *= amount2 ?? amount1;
                    break;
                case Axis.XYZ:
                    result.x *= amount1;
                    result.y *= amount2 ?? amount1;
                    result.z *= amount3 ?? amount1;
                    break;
            }

            return result;
        }

        public static Vector3 Multiply (this Vector3 target, Axis axis, Vector3 amount)
        {
            var result = target;
            switch (axis)
            {
                case Axis.X:
                    result.x *= amount.x;
                    break;
                case Axis.Y:
                    result.y *= amount.y;
                    break;
                case Axis.Z:
                    result.z *= amount.z;
                    break;
                case Axis.XY:
                    result.x *= amount.x;
                    result.y *= amount.y;
                    break;
                case Axis.XZ:
                    result.x *= amount.x;
                    result.z *= amount.z;
                    break;
                case Axis.YZ:
                    result.y *= amount.y;
                    result.z *= amount.z;
                    break;
                case Axis.XYZ:
                    result.x *= amount.x;
                    result.y *= amount.y;
                    result.z *= amount.z;
                    break;
            }

            return result;
        }
    }
}