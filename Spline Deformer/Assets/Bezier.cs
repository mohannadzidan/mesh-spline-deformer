using UnityEngine;

public static class Bezier
{

    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * p0 +
            2f * oneMinusT * t * p1 +
            t * t * p2;
    }
    public static float GetValue(float f0, float f1, float f2, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * f0 +
            2f * oneMinusT * t * f1 +
            t * t * f2;
    }


    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return
            2f * (1f - t) * (p1 - p0) +
            2f * t * (p2 - p1);
    }

    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float tInv = 1f - t;
        // Vector3 lerb01 = Vector3.Lerp(p0, p1, t);
        // Vector3 lerb12 = Vector3.Lerp(p1, p2, t);
        // Vector3 lerb23 = Vector3.Lerp(p2, p3, t);

        // Vector3 lerp01_12 = Vector3.Lerp(lerb01, lerb12, t);
        // Vector3 lerp12_23 = Vector3.Lerp(lerb12, lerb23, t);

        // Vector3 lerp = Vector3.Lerp(lerp01_12, lerp12_23, t);
        // return lerp;
        return
            tInv * tInv * tInv * p0 +
            3f * tInv * tInv * t * p1 +
            3f * tInv * t * t * p2 +
            t * t * t * p3;
    }


    public static float GetValue(float f0, float f1, float f2, float f3, float t)
    {
        t = Mathf.Clamp01(t);
        float OneMinusT = 1f - t;
        return
            OneMinusT * OneMinusT * OneMinusT * f0 +
            3f * OneMinusT * OneMinusT * t * f1 +
            3f * OneMinusT * t * t * f2 +
            t * t * t * f3;
    }
    public static Quaternion GetQuaterinion(Quaternion q0, Quaternion q1, Quaternion q2, Quaternion q3, float t)
    {
        t = Mathf.Clamp01(t);
        //float tInv = 1f - t;
        Quaternion lerb01 = Quaternion.Lerp(q0, q1, t);
        Quaternion lerb12 = Quaternion.Lerp(q1, q2, t);
        Quaternion lerb23 = Quaternion.Lerp(q2, q3, t);

        Quaternion lerp01_12 = Quaternion.Lerp(lerb01, lerb12, t);
        Quaternion lerp12_23 = Quaternion.Lerp(lerb12, lerb23, t);

        Quaternion lerp = Quaternion.Lerp(lerp01_12, lerp12_23, t);
        return lerp;
    }
    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            3f * oneMinusT * oneMinusT * (p1 - p0) +
            6f * oneMinusT * t * (p2 - p1) +
            3f * t * t * (p3 - p2);
    }
}