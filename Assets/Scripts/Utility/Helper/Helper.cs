using System.Collections;
using UnityEngine;

public static class Helper
{
    public static float SortByLowToHigh(float firstValue, float secondValue)
    {
        return firstValue.CompareTo(secondValue);
    }

    public static bool LayerMaskContainsLayer (this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }
}
