using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static bool InRange(Transform viewer, Transform target, float length, float fov)
    {
        // If the target is outside the viewer's sensor region, don't bother continuing
        if ((target.position - viewer.position).magnitude > length)
            return false;

        // We want to determine whether the angle between the player and enemy is less than half the enemy's fov
        // "return angle <= fov * 0.5f;"

        // We can solve for this angle using the dot product
        // a . b = ||a|| * ||b|| * cos(x)

        // We can simplify this equation because we know that a and b are normalized, so we can remove their magnitude
        // a . b = ||a|| * ||b|| * cos(x)
        // a . b = 1 * 1 * cos(x)
        // a . b = cos(x)
        // x = arccos(a . b)

        Vector3 targetDirection = (target.position - viewer.position).normalized;
        Vector3 viewerDirection = viewer.rotation * Vector3.forward;

        //float angle = Mathf.Acos(Vector3.Dot(targetDirection, viewerDirection)) * Mathf.Rad2Deg;
        //return angle <= fov * 0.5f;

        // We can compare a . b to cos(x) instead of solving for x to reduce complexity
        return Vector3.Dot(targetDirection, viewerDirection) > Mathf.Cos(Mathf.Deg2Rad * fov * 0.5f);
    }

    public static bool InRangeDebug(Transform viewer, Transform target, float length, float fov)
    {
        Vector3 front = viewer.transform.rotation.eulerAngles;
        Vector3 left = front;
        Vector3 right = front;
        Quaternion ql = new Quaternion();
        Quaternion qr = new Quaternion();
        left.y -= fov * 0.5f;
        right.y += fov * 0.5f;
        ql.eulerAngles = left;
        qr.eulerAngles = right;

        Color color = InRange(viewer, target, length, fov) ? Color.red : Color.green;
        Debug.DrawLine(viewer.position, viewer.position + ql * Vector3.forward * length, color);
        Debug.DrawLine(viewer.position, viewer.position + qr * Vector3.forward * length, color);
        return color == Color.red;
    }
}
