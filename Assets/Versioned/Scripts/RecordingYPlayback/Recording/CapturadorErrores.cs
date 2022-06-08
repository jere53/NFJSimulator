using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturadorErrores : MonoBehaviour
{
    private DatosEvaluacion _datosEvaluacionBuffer;

    private void Awake()
    {
        _datosEvaluacionBuffer = new DatosEvaluacion();
    }

    private void OnEnable()
    {
        RecordingManager.Instance.OnCaptureSnapshot += CargarSnapshot;
    }

    private void OnDisable()
    {
        RecordingManager.Instance.OnCaptureSnapshot -= CargarSnapshot;
    }

    private void CargarSnapshot(Recorder recorder)
    {
        recorder.capturaDatosEvaluacion = _datosEvaluacionBuffer;
        _datosEvaluacionBuffer = new DatosEvaluacion();
    }

    public void AddCapturaAceleracion(Tuple<float, float, float, float, float> infraccion)
    {
        _datosEvaluacionBuffer.DatosCriterioAceleracion.Infracciones.Add(infraccion);
    }

    public void AddCapturaAccidente(float time)
    {
        _datosEvaluacionBuffer.DatosCriterioEvitarAccidentes.Item1.Add(time);
    }

    public void AddCapturaNafta(float litrosConsumidos)
    {
        _datosEvaluacionBuffer.DatosCriterioNafta.LitrosConsumidos = litrosConsumidos;
    }

    public void AddCapturaSemaforo(float timestamp)
    {
        _datosEvaluacionBuffer.DatosCriterioRespetarSemaforos.Add(timestamp);
    }

    public void AddCapturaRpm(Tuple<float, int, int, int, bool> infraccion)
    {
        _datosEvaluacionBuffer.DatosCriterioRpm.Infracciones.Add(infraccion);
    }

    public void AddCapturaTiempo(float timestamp)
    {
        Tuple<TimeSpan, TimeSpan> tiempo =
            new Tuple<TimeSpan, TimeSpan>(TimeSpan.FromSeconds(timestamp), TimeSpan.FromSeconds(timestamp));
        _datosEvaluacionBuffer.DatosCriterioTiempo = tiempo;
    }

    public void AddCapturaVelocidad(Tuple<float, float, bool> infraccion)
    {
        _datosEvaluacionBuffer.DatosCriterioVelocidad.Infracciones.Add(infraccion);
    }

    public void AddCapturaVolantazo(Tuple<float, float, float> infraccion)
    {
        _datosEvaluacionBuffer.DatosCriterioVolantazos.Infracciones.Add(infraccion);
    }

    public DatosEvaluacion ConsumeBufferedEvalData()
    {
        var res = _datosEvaluacionBuffer;
        _datosEvaluacionBuffer = new DatosEvaluacion();
        return res;
    }
}
