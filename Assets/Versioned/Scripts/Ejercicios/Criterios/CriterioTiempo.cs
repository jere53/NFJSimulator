using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriterioTiempo : MonoBehaviour, ICriterio
{
    private bool _cronometroActivado;

    private float _tiempoActual;

    public int minutosMaximosPermitidos;

    public int segundosMaximosPermitidos;

    // Update is called once per frame
    void Update()
    {
        if (_cronometroActivado)
        {
            _tiempoActual += Time.deltaTime;
        }
    }

    public void ComenzarEvaluacion()
    {
        _tiempoActual = 0;
        _cronometroActivado = true;
    }

    public void ConcluirEvaluacion()
    {
        _cronometroActivado = false;
    }

    public void PresentarEvaluacion()
    {
        string resultadoEvaluacion = "";
        
        TimeSpan tiempoEvaluado = TimeSpan.FromSeconds(_tiempoActual);
        
        TimeSpan tiempoMaximo = TimeSpan.FromSeconds(minutosMaximosPermitidos * 60 + segundosMaximosPermitidos);

        if (minutosMaximosPermitidos * 60 + segundosMaximosPermitidos > 0)
        {
            //si el tiempo maximo es 0, es porque no se quiere un tiempo maximo
            if (_tiempoActual > (minutosMaximosPermitidos * 60 + segundosMaximosPermitidos))
                resultadoEvaluacion = "--------REPROBADO--------" + '\n';
            else
                resultadoEvaluacion = "--------APROBADO--------" + '\n';

            resultadoEvaluacion += "Tiempo Requerido para aprobar: " + tiempoMaximo.Minutes + "\'" + tiempoMaximo.Seconds
                                   + "\"" + '\n';
        }

        resultadoEvaluacion += "Tiempo utilizado por el Evaluado: " + tiempoEvaluado.Minutes + "\'" + tiempoEvaluado.Seconds 
                               +"\"" + '\n';

        Debug.Log(resultadoEvaluacion);
    }
    
    public void Remover()
    {
        Destroy(this);
    }
}
