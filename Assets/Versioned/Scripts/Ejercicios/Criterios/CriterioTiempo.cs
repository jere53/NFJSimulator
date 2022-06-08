using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriterioTiempo : MonoBehaviour, ICriterio
{
    private float _tiempoActual;

    public int minutosMaximosPermitidos;

    public int segundosMaximosPermitidos;

    private bool isRecording;
    private CapturadorErrores _capturadorErrores;
    

    // Update is called once per frame
    void Update()
    {
        _tiempoActual += Time.deltaTime;

        if (isRecording)
        {
            if (_tiempoActual > (minutosMaximosPermitidos * 60) + segundosMaximosPermitidos)
            {
                _capturadorErrores.AddCapturaTiempo(_tiempoActual);
                DisableRecording();
            }
        }
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
    
    public void EnableRecording(CapturadorErrores capturadorErrores)
    {
        if(!capturadorErrores) return;
        _capturadorErrores = capturadorErrores;
        isRecording = true;
    }

    public void DisableRecording()
    {
        isRecording = false;
        _capturadorErrores = null;
    }


}
