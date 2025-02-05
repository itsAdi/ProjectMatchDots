using UnityEngine;

[ExecuteAlways]
public class PointOnRect : MonoBehaviour
{
    public Vector2 Point = new (1f, 6f);
    public Rect Box = new(2f, 4f, 3f, 5f);

    private Vector2 m_ClosestPoint;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Box.center, Box.size);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(Point, 0.05f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(Point, m_ClosestPoint);
    }

    private void Update()
    {
        m_ClosestPoint = ClosestPointToRectEdges(Box, Point);
    }

    private static Vector2 ClosestPointToRectEdges(Rect box, Vector2 point)
    {
        var vertex0 = box.min;
        var vertex1 = new Vector2(box.xMin, box.yMax);
        var vertex2 = box.max;
        var vertex3 = new Vector2(box.xMax, box.yMin);
 
        // Find closest point/edge.
        var closestPoint = Vector2.zero;
        var closestSqrDistance = float.MaxValue;
        CheckBestEdge(vertex0, vertex1, point, ref closestPoint, ref closestSqrDistance);
        CheckBestEdge(vertex1, vertex2, point, ref closestPoint, ref closestSqrDistance);
        CheckBestEdge(vertex2, vertex3, point, ref closestPoint, ref closestSqrDistance);
        CheckBestEdge(vertex3, vertex0, point, ref closestPoint, ref closestSqrDistance);

        return closestPoint;
    }

    private static void CheckBestEdge(
        Vector2 edgeStart, Vector2 edgeEnd, Vector2 point,
        ref Vector2 bestPoint, ref float bestSqrDistance)
    {
        var edgePoint = edgeStart;
 
        var edgeSegment = edgeEnd - edgeStart;
        var length = Vector2.SqrMagnitude(edgeSegment);
        if (length > Mathf.Epsilon)
            edgePoint = edgeStart + Mathf.Clamp01(Vector2.Dot(edgeSegment, point - edgeStart) / length) * edgeSegment;

        var sqrDistance = Vector2.SqrMagnitude(edgePoint - point);
        if (sqrDistance < bestSqrDistance)
        {
            bestPoint = edgePoint;
            bestSqrDistance = sqrDistance;
        }
    }
}