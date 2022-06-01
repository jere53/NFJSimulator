using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrieferModel : MonoBehaviour
{
    private DatosEvaluacion _datosEvaluacion;

    public void CargarDatosEvaluacion(string path)
    {
        _datosEvaluacion = DatosEvaluacion.CargarDeDisco(path);
        _datosEvaluacion.Presentar();
    }

    public DatosCriterioAceleracion GetDatosAceleracion()
    {
        return _datosEvaluacion.DatosCriterioAceleracion;
    }

    public Tuple< List<float>, List<Tuple<float, int>>> GetDatosAccidentes()
    {
        return _datosEvaluacion.DatosCriterioEvitarAccidentes;
    }

    public DatosCriterioNafta GetDatosConsumo()
    {
        return _datosEvaluacion.DatosCriterioNafta;
    }

    public List<float> GetDatosSemaforo()
    {
        return _datosEvaluacion.DatosCriterioRespetarSemaforos;
    }

    public DatosCriterioRpm GetDatosRpm()
    {
        return _datosEvaluacion.DatosCriterioRpm;
    }

    public Tuple<TimeSpan, TimeSpan> GetDatosTiempo()
    {
        return _datosEvaluacion.DatosCriterioTiempo;
    }

    public DatosCriterioVelocidad GetDatosVelocidad()
    {
        return _datosEvaluacion.DatosCriterioVelocidad;
    }

    public DatosCriterioVolantazos GetDatosVolantazos()
    {
        return _datosEvaluacion.DatosCriterioVolantazos;
    }
}
