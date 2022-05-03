using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriterioTiempo : MonoBehaviour, ICriterio
{
    private float _tiempoActual;

    public int minutosMaximosPermitidos;

    public int segundosMaximosPermitidos;

    // Update is called once per frame
    void Update()
    {
        _tiempoActual += Time.deltaTime;
    }

    public void ComenzarEvaluacion()
    {
        _tiempoActual = 0;
        enabled = true;
    }

    public void ConcluirEvaluacion()
    {
        enabled = false;
    }

    public void ObtenerDatosEvaluacion(ref DatosEvaluacion datosEvaluacion)
    {
        
        string resultadoEvaluacion = "";
        
        TimeSpan tiempoEvaluado = TimeSpan.FromSeconds(_tiempoActual);
        
        TimeSpan tiempoMaximo = TimeSpan.FromSeconds(minutosMaximosPermitidos * 60 + segundosMaximosPermitidos);
        
        datosEvaluacion.DatosCriterioTiempo = new Tuple<TimeSpan, TimeSpan>(tiempoEvaluado, tiempoMaximo);
    }
    
    public void Remover()
    {
        Destroy(this);
    }
}
