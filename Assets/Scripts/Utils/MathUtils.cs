
using UnityEngine;

class MathUtils
{
    public static Vector2 Rotate(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    public static float GetAngle2D(Vector2 a, Vector2 b)
    {
        float dot = Vector2.Dot(a, b);
        float mags = a.magnitude * b.magnitude;

        return Mathf.Acos(dot / mags);
    }

    public static bool IsPointInEllipse(Vector2 point, Vector2 ellipseCentre, float minorRadius, float majorRadius)
    {
        float major = Mathf.Pow((point.x - ellipseCentre.x) / majorRadius, 2f);

        float minor = Mathf.Pow((point.y - ellipseCentre.y) / minorRadius, 2f);

        return major + minor <= 1f;
    }

}
