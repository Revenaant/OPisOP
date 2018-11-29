using UnityEngine;
using System.Collections;

public static class GizmosHelper
{
    public static void DrawWithColor(Color color, System.Action function)
    {
        Color previous = Gizmos.color;
        Gizmos.color = color;
        function();
        Gizmos.color = previous;
    }

    public static void DrawGizmoArrow(Vector3 position, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.DrawRay(position, direction);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Gizmos.DrawRay(position + direction, right * arrowHeadLength);
        Gizmos.DrawRay(position + direction, left * arrowHeadLength);
    }

    public static void DrawGizmoArrow(Vector3 position, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Color previous = Gizmos.color;
        Gizmos.color = color;

        DrawGizmoArrow(position, direction, arrowHeadLength, arrowHeadAngle);

        Gizmos.color = previous;
    }

    public static void DrawGizmoLineArrow(Vector3 from, Vector3 to, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.DrawLine(from, to);

        Vector3 direction = to - from;
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Gizmos.DrawRay(to, right * arrowHeadLength);
        Gizmos.DrawRay(to, left * arrowHeadLength);
    }

    public static void DrawGizmoLineArrow(Vector3 from, Vector3 to, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Color previous = Gizmos.color;
        Gizmos.color = color;

        DrawGizmoLineArrow(from, to, arrowHeadLength, arrowHeadAngle);

        Gizmos.color = previous;
    }

    public static void DrawDebugArrow(Vector3 position, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Debug.DrawRay(position, direction);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Debug.DrawRay(position + direction, right * arrowHeadLength);
        Debug.DrawRay(position + direction, left * arrowHeadLength);
    }

    public static void DrawDebugArrow(Vector3 position, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Debug.DrawRay(position, direction, color);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Debug.DrawRay(position + direction, right * arrowHeadLength, color);
        Debug.DrawRay(position + direction, left * arrowHeadLength, color);
    }

    public static void DrawPositions(Vector3 start, Vector3[] positions)
    {
        if (positions != null && positions.Length != 0)
        {
            Gizmos.DrawLine(start, positions[0]);
            for (int i = 0; i < positions.Length; i++)
            {
                if (i + 1 >= positions.Length) continue;

                Gizmos.DrawLine(positions[i], positions[i + 1]);
            }
        }
    }

    public static void DrawPositions(Vector3 start, Vector3[] positions, Color color)
    {
        if (positions != null && positions.Length != 0)
        {
            Color previous = Gizmos.color;
            Gizmos.color = color;

            Gizmos.DrawLine(start, positions[0]);
            for (int i = 0; i < positions.Length; i++)
            {
                if (i + 1 >= positions.Length) continue;

                Gizmos.DrawLine(positions[i], positions[i + 1]);
            }

            Gizmos.color = previous;
        }
    }

    public static void DrawPositions(Vector3[] positions)
    {
        if (positions != null && positions.Length != 0)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                if (i + 1 >= positions.Length) continue;

                Gizmos.DrawLine(positions[i], positions[i + 1]);
            }
        }
    }

    public static void DrawPositions(Vector3[] positions, Color color)
    {
        if (positions != null && positions.Length != 0)
        {
            Color previous = Gizmos.color;
            Gizmos.color = color;

            for (int i = 0; i < positions.Length; i++)
            {
                if (i + 1 >= positions.Length) continue;

                Gizmos.DrawLine(positions[i], positions[i + 1]);
            }

            Gizmos.color = previous;
        }
    }
}