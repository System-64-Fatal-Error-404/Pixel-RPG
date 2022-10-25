using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier : MonoBehaviour
{
    public List<Transform> points;

    void Update()
    {
        float t = Mathf.Cos(Time.realtimeSinceStartup) * 0.5f + 0.5f;
        transform.position = Hermite(points[0].position, points[1].position,
            points[2].position, points[3].position, t);
    }

    private Vector3 Hermite(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 r0 = -1.0f * p0 + 3.0f * p1 + -3.0f * p2 + 1.0f * p3;
        Vector3 r1 = 3.0f * p0 + -6.0f * p1 + 3.0f * p2;
        Vector3 r2 = -3.0f * p0 + 3.0f * p1;
        Vector3 r3 = p0;

        return (t * t * t) * r0 + (t * t) * r1 + t * r2 + r3;
    }

	private Vector3 Decasteljau(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 A = Vector3.Lerp(p0, p1, t);
        Vector3 B = Vector3.Lerp(p1, p2, t);
        Vector3 C = Vector3.Lerp(p2, p3, t);

		Vector3 D = Vector3.Lerp(A, B, t);
		Vector3 E = Vector3.Lerp(B, C, t);

        Vector3 F = Vector3.Lerp(D, E, t);

        return F;
	}
}
