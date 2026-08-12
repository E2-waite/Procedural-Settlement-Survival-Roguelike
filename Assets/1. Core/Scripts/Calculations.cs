using UnityEngine;

public static class Calculations
{
    public static Quaternion SnappedRotation(Vector3 dir)
    {
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        angle = Mathf.Round(angle / 45f) * 45f;
        return Quaternion.Euler(0, angle, 0);
    }
}
