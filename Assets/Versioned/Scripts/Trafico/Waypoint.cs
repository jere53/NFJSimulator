using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Waypoint : MonoBehaviour //Tiene que ser monobehaviour? Se puede poner en un gameobject si no lo es?
{
    public List<Waypoint> proximosWaypoints = new List<Waypoint>();
    public List<float> probabilidadWaypoints = new List<float>();

    private void Start()
    {
        if (proximosWaypoints.Count < 1) return;
        
        float suma = 0;
        foreach (float prob in probabilidadWaypoints)
        {
            suma += prob;
        }

        if (suma > 1.00001f || suma < 0.99999f) 
            Debug.LogWarning("La suma de las probabilidades del " + this + " no es igual a 1");

    }


    public Waypoint ObtenerSiguiente(float probabilidad)
    {
        if (proximosWaypoints.Count == 0) return null;
        float probAcum = 0;
        int proxWp = 0;
        foreach (var prob in probabilidadWaypoints)
        {
            probAcum += prob;
            if (probabilidad < probAcum)
            {
                return proximosWaypoints[proxWp];
            }

            proxWp++;
        }

        
        Debug.Log("devuelve Waypoint fallback");
        //fallback
        return proximosWaypoints[0];

    }
    
    //Editor-------------------------------------
    public void AgregarWaypointSiguiente(Waypoint waypoint, float probabilidad)
    {
        int i;
        for (i = 0; i < proximosWaypoints.Count; i++)
        {
            if (probabilidad < probabilidadWaypoints[i])
                break;
        }
        proximosWaypoints.Insert(i, waypoint);
        probabilidadWaypoints.Insert(i, probabilidad);
    }

    public void BorrarSiguiente(int id)
    {
        for (int i = 0; i < proximosWaypoints.Count; i++)
        {
            if (proximosWaypoints[i].GetInstanceID() == id)
            {
                Debug.Log("Borrado desde el waypoint: " + this);
                proximosWaypoints.RemoveAt(i);
                probabilidadWaypoints.RemoveAt(i);
            }
        }
    }
    //------------------------------------

}
