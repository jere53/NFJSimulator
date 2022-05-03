using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoadMarker : MonoBehaviour
{
    [SerializeField] private Waypoint waypointActual;
    [SerializeField] private float distanciaLlegoDestino;
    [SerializeField] private GameObject plano;
    [NonSerialized] public bool estaVolviendo;
    private DatosEvaluacion _datosEvaluacion = new DatosEvaluacion();

    void Update()
    {
        if (waypointActual != null)
        {
            plano.transform.position = waypointActual.transform.position;
            plano.transform.rotation = waypointActual.transform.rotation;
            if (distancia(waypointActual.transform) < distanciaLlegoDestino)
            {
                waypointActual = waypointActual.ObtenerSiguiente(Random.value);
            }
        }
        else
        {
            plano.GetComponent<Renderer>().enabled = false;
            if (estaVolviendo) //ejercicio completado
            {
                ICriterio[] criterios = GetComponents<ICriterio>();
                foreach (var criterio in criterios)
                {
                    criterio.ConcluirEvaluacion();
                    criterio.ObtenerDatosEvaluacion(ref _datosEvaluacion);
                    criterio.Remover();
                }

                _datosEvaluacion.Presentar();
                estaVolviendo = false;
                enabled = false;
            } 
        }
    }

    public void habilitarPlano()
    {
        plano.GetComponent<Renderer>().enabled = true;
    }
    
    public float distancia (Transform objeto)
    {
        Vector3 destino = objeto.transform.position;
        destino.y = 0;

        Vector3 posActual = transform.position;
        posActual.y = 0;

        return Vector3.Magnitude(destino - posActual);
    }

    public void setDestino(Waypoint waypoint)
    {
        waypointActual = waypoint;
    }
    
}
