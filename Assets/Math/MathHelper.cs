using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathHelper
{
   public static Vector2 AngleToVector2(float angle) {
        float rads = angle * Mathf.Deg2Rad;
        Vector2 vector = new Vector2(Mathf.Cos(rads), Mathf.Sin(rads));
        return vector;
    }
}
