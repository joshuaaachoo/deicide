using UnityEngine;
public static class GuoUtility
{
    public static float GoldenMean(Vector3 attacker, Vector3 target)
    {
        float d = Vector3.Distance(attacker, target);
        if (d <= 0.2f || d >= 6f) return 0.25f;

        float normalized = Mathf.InverseLerp(0.2f, 6f, d);
        float peak = Mathf.InverseLerp(0.2f, 6f, 2f);
        float curve = Mathf.Sin((normalized - peak) * Mathf.PI);
        return Mathf.Lerp(0.25f, 1f, curve);
    }
}
