using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PrimitiveTypeExtensions
{
    public static bool AlmostEquals(this float originalValue, float comparerValue, float precision)
    {
        return (Mathf.Abs(originalValue - comparerValue) <= precision);
    }
}
