using UnityEngine;

public class FB_LineRenderer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public int resolution = 30;
    public float timeStep = 0.05f;

    public void DrawTrajectory(Vector3 startPos, Vector3 initialVelocity)
    {
        Vector3[] points = new Vector3[2];
        points[0] = startPos;
        points[1] = initialVelocity;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPositions(points);
    }

    public void Clear()
    {
        lineRenderer.positionCount = 0;
    }
}
