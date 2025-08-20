using UnityEngine;

public class PathWaypoints : MonoBehaviour
{
    [Tooltip("رتّبي النقاط: قدّام → جنب → قدّام، وخلي آخر نقطة داخل منطقة الهدف")]
    public Transform[] waypoints;

    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Length; i++)
        {
            var a = waypoints[i];
            if (!a) continue;

            Gizmos.DrawSphere(a.position, 0.15f);

            if (i + 1 < waypoints.Length && waypoints[i + 1])
                Gizmos.DrawLine(a.position, waypoints[i + 1].position);
        }
    }
}
