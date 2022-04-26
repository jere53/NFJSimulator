using UnityEditor;
using UnityEngine;

namespace Trafico
{

    [InitializeOnLoad]
    public class WaypointEditor
    {
        [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
        public static void OnDrawSceneGizmo(Waypoint waypoint, GizmoType gizmoType)
        {
            if ((gizmoType & GizmoType.Selected) != 0)
                Gizmos.color = Color.yellow;
            else
                Gizmos.color = Color.yellow * 0.5f;
            Gizmos.DrawSphere(waypoint.transform.position, 1f);

            if (waypoint.proximosWaypoints.Count != 0)
            {
                for (int i = 0; i < waypoint.proximosWaypoints.Count; i++)
                {
                    Gizmos.color = Color.red * waypoint.probabilidadWaypoints[i]; // cuanto más probabilidad tenga, más fuerte será el color rojo
                    DibujarFlecha(waypoint.transform.position, waypoint.proximosWaypoints[i].transform.position - waypoint.transform.position, 1.5f);
                }
            }
        }
        
        public static void DibujarFlecha(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.DrawRay(pos, direction);
            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }
        
        
    }
}