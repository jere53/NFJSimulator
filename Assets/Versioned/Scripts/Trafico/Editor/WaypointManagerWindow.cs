using UnityEditor;
using UnityEngine;
public class WaypointManagerWindow : EditorWindow
{
    [MenuItem("Tools/Editor de Waypoints")]
    public static void Open()
    {
        GetWindow<WaypointManagerWindow>("Waypoints Manager");
    }

    public Transform waypointRoot;
    public float probabilidad;

    private void OnGUI()
    {
        SerializedObject obj = new SerializedObject(this);

        EditorGUILayout.PropertyField(obj.FindProperty("waypointRoot"));
        EditorGUILayout.PropertyField(obj.FindProperty("probabilidad"));

        if (waypointRoot == null)
        {
            EditorGUILayout.HelpBox("Se debe seleccionar un tranform origen", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.BeginVertical("box");
            DrawButtons();
            EditorGUILayout.EndVertical();
        }
        
        obj.ApplyModifiedProperties();

    }

    void DrawButtons()
    {
        if (Selection.activeGameObject != null && Selection.activeGameObject.transform == waypointRoot)
        {
            if (GUILayout.Button("Crear waypoint"))
            {
                CreateWaypoint();
            }
        }
        else if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Waypoint>())
        {
            if (GUILayout.Button("Crear Waypoint Siguiente"))
            {
                CrearWaypointSiguiente();
            }
            if (GUILayout.Button("Crear Waypoint Anterior"))
            {
                CrearWaypointAnterior();
            }

            if (GUILayout.Button("Borrar Waypoint"))
            {
                BorrarWaypoint();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Seleccione el WaypointRoot o algun waypoint para empezar a generar", MessageType.Info);
        }
        
        
    }
    
    
    void CreateWaypoint() //Cambiar y hacer que lo cree anterior al primero
    {
        int hijos = waypointRoot.childCount;
        GameObject waypointObject = new GameObject("Waypoint " + hijos, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot, false);
        Waypoint waypoint = waypointObject.GetComponent<Waypoint>();

        if (hijos > 1)
        {
            Debug.Log("anterior: " + waypointRoot.GetChild(waypointRoot.childCount - 2).GetComponent<Waypoint>() + "    actual: " + waypoint );
            waypointRoot.GetChild(hijos - 2).GetComponent<Waypoint>().AgregarWaypointSiguiente(waypoint , probabilidad);
        }

        Selection.activeGameObject = waypoint.gameObject;
    }

    void CrearWaypointSiguiente()
    {
        GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot, false);
        
        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();

        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        waypointObject.transform.position = selectedWaypoint.transform.position + Vector3.forward; // Lo muevo un poco hacia adelante asi no se pone justo en la misma posicion del seleccionado
        waypointObject.transform.forward = selectedWaypoint.transform.forward;
        
        selectedWaypoint.AgregarWaypointSiguiente(newWaypoint, probabilidad);
        
        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex() + 1);
        
        Selection.activeGameObject = newWaypoint.gameObject;
    }

    void CrearWaypointAnterior()
    {
        GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot, false);
        
        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();

        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        waypointObject.transform.position = selectedWaypoint.transform.position - Vector3.forward; // Lo muevo un poco hacia atras asi no se pone justo en la misma posicion del seleccionado
        waypointObject.transform.forward = selectedWaypoint.transform.forward;
        
        newWaypoint.AgregarWaypointSiguiente(selectedWaypoint, probabilidad);
        
        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());

        Selection.activeGameObject = newWaypoint.gameObject;
    }

    void BorrarWaypoint()
    {
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        var objects = FindObjectsOfType<Waypoint>();
        var objectCount = objects.Length;
        foreach (var obj in objects)
        {
            obj.BorrarSiguiente(selectedWaypoint.GetInstanceID());
        }
        
        DestroyImmediate(selectedWaypoint.gameObject);
    }
}
