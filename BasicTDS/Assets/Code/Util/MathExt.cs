using UnityEngine;

public static class MathExt
{
    public static float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        Vector2 c = new Vector2(a.x - b.x, a.y - b.y);
        c.Normalize();
        return Mathf.Atan2(c.y, c.x) * Mathf.Rad2Deg;
    }

    public static Vector2 RotateVector2(Vector2 v, float degrees)
    {
        float rads = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(rads);
        float cos = Mathf.Cos(rads);
        return new Vector2(
            (cos * v.x) - (sin * v.y),
            (sin * v.x) + (cos * v.y));
    }
}
