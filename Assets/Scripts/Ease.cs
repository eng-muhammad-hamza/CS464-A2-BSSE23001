using UnityEngine;

public static class Ease
{
    public static float OutCubic(float t) { t = Mathf.Clamp01(t); return 1f - Mathf.Pow(1f - t, 3f); }

    public static float OutBack(float t, float s = 1.70158f)
    {
        t = Mathf.Clamp01(t);
        float c3 = s + 1f;
        float u = t - 1f;
        return 1f + c3 * u * u * u + s * u * u;
    }
}
