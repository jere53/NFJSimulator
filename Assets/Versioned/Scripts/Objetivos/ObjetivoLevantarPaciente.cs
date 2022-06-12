using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VehiclePhysics;

public class ObjetivoLevantarPaciente : MonoBehaviour
{
    [SerializeField] private Waypoint camidoIda;
    [SerializeField] private Waypoint camidoVuelta;
    [SerializeField] private RoadMarker vehiculo;
    [SerializeField] private GameObject ejercicioRoot;
    
    private Material material;
    private Coroutine c1;
    private Color verde = new Color(0, 1, 0, 0.5f);
    private Color rojo = new Color(1, 0, 0, 0.5f);

    private bool cumplioObjetivo;
    public void Comenzar()
    {
        ejercicioRoot.SetActive(true);
        material = GetComponent<Renderer>().material;
        vehiculo.estaVolviendo = false;
        material.color = rojo;
        vehiculo.gameObject.SetActive(false);
        vehiculo.transform.position = camidoIda.transform.position;
        vehiculo.transform.rotation = camidoIda.transform.rotation;
        vehiculo.gameObject.SetActive(true);
        vehiculo.enabled = true;
        vehiculo.setDestino(camidoIda);
        vehiculo.habilitarPlano();
        var criterios = vehiculo.GetComponents<ICriterio>();
        foreach (var c in criterios)
        {
            c.ComenzarEvaluacion();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CumplidorObjetivoEjercicio"))
        {
            material.color = verde;
            c1 = StartCoroutine(StartCountdown());

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CumplidorObjetivoEjercicio"))
        {
            material.color = rojo;
            StopCoroutine(c1);
        }
    }

    public IEnumerator StartCountdown(float countdownValue = 10)
    {
        float currCountdownValue = countdownValue;
        while (currCountdownValue > 0)
        {
            Debug.Log("Countdown: " + currCountdownValue);
            yield return new WaitForSeconds(1.0f);
            currCountdownValue--;
        }
        CumplioObjetivo();
    }

    public void CumplioObjetivo()
    {
        Debug.Log("Paciente Levantado");
        cumplioObjetivo = true;
        vehiculo.setDestino(camidoVuelta);
        vehiculo.estaVolviendo = true;
        vehiculo.habilitarPlano();
    }
    
}
