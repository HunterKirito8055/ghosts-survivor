using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MathHelper
{

    public static class VectorHelper
    {
        public static Vector3 GetDirectionAtAngle(float angle)
        {
            angle = Mathf.Deg2Rad * angle;
            return new Vector3(
                (Mathf.Cos(angle) - Mathf.Sin(angle)),
                0,
                (Mathf.Sin(angle) + Mathf.Cos(angle)));
        }

        public static Vector3 Rotate(Vector3 vector, float angle, bool clockwise)
        {
            angle = Mathf.Deg2Rad * angle;
            if (clockwise)
                angle = 2 * Mathf.PI - angle;

            float xVal = vector.x * Mathf.Cos(angle) - vector.z * Mathf.Sin(angle);
            float zVal = vector.x * Mathf.Sin(angle) + vector.z * Mathf.Cos(angle);
            return new Vector3(xVal, 0, zVal);
        }

        public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t, ParabolaPlane parabolaPlane)
        {

            Func<float, float> f = x => -4 * height * x * x + 4 * height * x;
            Vector3 mid = Vector3.Lerp(start, end, t);
            float a, b;
            switch (parabolaPlane)
            {
                case ParabolaPlane.X:
                case ParabolaPlane.X_Revrs:
                    a = start.x;
                    b = end.x;
                    break;
                case ParabolaPlane.Y:
                default:
                    a = start.y;
                    b = end.y;
                    break;
            }
            float differential = 0;
            switch (parabolaPlane)
            {
                case ParabolaPlane.X:
                    differential = f(t) + Mathf.Lerp(a, b, t);
                    return new Vector3(differential, mid.y, mid.z);
                case ParabolaPlane.X_Revrs:
                    differential = -f(t) - Mathf.Lerp(a, b, t);
                    return new Vector3(-differential, mid.y, mid.z);
                case ParabolaPlane.Y:
                default:
                    differential = f(t) + Mathf.Lerp(a, b, t);
                    return new Vector3(mid.x, differential, mid.z);
            }
        }
    }
    public enum ParabolaPlane
    {
        Y,
        X,
        X_Revrs
    }
}
