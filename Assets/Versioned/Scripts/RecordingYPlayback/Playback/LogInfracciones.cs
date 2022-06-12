using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogInfracciones : MonoBehaviour
{
    public int maxPopupAmount;

    public GameObject popUpPrefab;

    public Reproductor Reproductor;
    private void MostrarInfraccion(DatosEvaluacion datosEvaluacion)
    {
        var infraccionesAceleracion = datosEvaluacion.DatosCriterioAceleracion.Infracciones;
        var infraccionesVelocidad = datosEvaluacion.DatosCriterioVelocidad.Infracciones;
        var infraccionesVolantazos = datosEvaluacion.DatosCriterioVolantazos.Infracciones;
        var infraccionesAccidentesPeatones = datosEvaluacion.DatosCriterioEvitarAccidentes.Item1;
        var infraccionesAccidentesVehiculos = datosEvaluacion.DatosCriterioEvitarAccidentes.Item2;
        var infraccionesRpm = datosEvaluacion.DatosCriterioRpm.Infracciones;
        var infraccionesSemaforos = datosEvaluacion.DatosCriterioRespetarSemaforos;
        var infraccionNafta = datosEvaluacion.DatosCriterioNafta;
        var infraccionTiempo = datosEvaluacion.DatosCriterioTiempo;


        string textoInfraccion = "Exceso en la Aceleracion al momento ";
        foreach (var infAcc in infraccionesAceleracion)
        {
            InstanciarPopUp(textoInfraccion + TimeSpan.FromSeconds(infAcc.Item1));
        }

        textoInfraccion = "Exceso de Velocidad al momento ";
        foreach (var infVec in infraccionesVelocidad)
        {
            if (infVec.Item3)
            {
                InstanciarPopUp(textoInfraccion + TimeSpan.FromSeconds(infVec.Item1));
            }
        }

        textoInfraccion = "Volantazo excesivo al momento ";
        foreach (var infVol in infraccionesVolantazos)
        {
            InstanciarPopUp(textoInfraccion + TimeSpan.FromSeconds(infVol.Item1));
        }

        textoInfraccion = "Se impacto contra un Peaton al momento ";
        foreach (var infPet in infraccionesAccidentesPeatones)
        {
            InstanciarPopUp(textoInfraccion + TimeSpan.FromSeconds(infPet));
        }

        textoInfraccion = "Se choco contra un vehiculo al momento ";
        foreach (var infVel in infraccionesAccidentesVehiculos)
        {
            InstanciarPopUp(textoInfraccion + TimeSpan.FromSeconds(infVel.Item1));
        }

        textoInfraccion = "Infraccion en las RPMs al momento ";
        foreach (var infRpm in infraccionesRpm)
        {
            if (infRpm.Item5)
            {
                InstanciarPopUp(textoInfraccion + TimeSpan.FromSeconds(infRpm.Item1));
            }
        }

        textoInfraccion = "Se paso un semaforo en rojo al momento ";
        foreach (var infSem in infraccionesSemaforos)
        {
            InstanciarPopUp(textoInfraccion + TimeSpan.FromSeconds(infSem));
        }

        textoInfraccion = "Se excedio el maximo de combustible";
        if (infraccionNafta.LitrosConsumidos > infraccionNafta.ObjetivoListrosConsumidos)
        {
            InstanciarPopUp(textoInfraccion);
        }

        textoInfraccion = "Se excedio el maximo de tiempo";
        if (infraccionTiempo.Item1.TotalSeconds > infraccionTiempo.Item2.TotalSeconds)
        {
            InstanciarPopUp(textoInfraccion);
        }

        int trap = 64;
        while (transform.childCount > maxPopupAmount && trap > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
            trap--;
        }
    }

    private void InstanciarPopUp(string text)
    {
        var popUp = Instantiate(popUpPrefab, transform, false);
        popUp.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }
    
    private void OnEnable()
    {
        Reproductor.OnMostrarInfraccion += MostrarInfraccion;
    }

    private void OnDisable()
    {
        Reproductor.OnMostrarInfraccion -= MostrarInfraccion;
    }
}
