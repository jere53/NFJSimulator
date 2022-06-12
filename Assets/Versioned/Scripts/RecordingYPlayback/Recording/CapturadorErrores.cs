using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturadorErrores : MonoBehaviour
{
    private DatosEvaluacion _datosEvaluacionBuffer;

    private ICriterio[] _criterios;

    private void Awake()
    {

    }

    private void OnEnable()
    {
        RecordingManager.Instance.OnCaptureSnapshot += CargarSnapshot;
        _datosEvaluacionBuffer = new DatosEvaluacion();
        InitBuffer();
        _criterios = GetComponents<ICriterio>();
        foreach (var criterio in _criterios)
        {
            criterio.EnableRecording(this);
        }
    }

    private void InitBuffer()
    {
        _datosEvaluacionBuffer.DatosCriterioAceleracion = new DatosCriterioAceleracion();
        _datosEvaluacionBuffer.DatosCriterioNafta = new DatosCriterioNafta();
        _datosEvaluacionBuffer.DatosCriterioVelocidad = new DatosCriterioVelocidad();
        _datosEvaluacionBuffer.DatosCriterioTiempo = new Tuple<TimeSpan, TimeSpan>(TimeSpan.Zero, TimeSpan.Zero);
        _datosEvaluacionBuffer.DatosCriterioRpm = new DatosCriterioRpm();
        _datosEvaluacionBuffer.DatosCriterioVolantazos = new DatosCriterioVolantazos();
        _datosEvaluacionBuffer.DatosCriterioEvitarAccidentes =
            new Tuple<List<float>, List<Tuple<float, int>>>(new List<float>(), new List<Tuple<float, int>>());
        _datosEvaluacionBuffer.DatosCriterioRespetarSemaforos = new List<float>();
    }

    private void OnDisable()
    {
        RecordingManager.Instance.OnCaptureSnapshot -= CargarSnapshot;
        foreach (var criterio in _criterios)
        {
            criterio.DisableRecording();
        }
    }

    private void CargarSnapshot(Recorder recorder)
    {
        recorder.capturaDatosEvaluacion = ConsumeBufferedEvalData();
    }

    public void AddCapturaAceleracion(Tuple<float, float, float, float, float> infraccion)
    {
        _datosEvaluacionBuffer.DatosCriterioAceleracion.Infracciones.Add(infraccion);
    }

    public void AddCapturaAccidente(float time)
    {
        Debug.Log("Accidente Capturado");
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

    private DatosEvaluacion ConsumeBufferedEvalData()
    {
        var res = _datosEvaluacionBuffer;
        _datosEvaluacionBuffer = new DatosEvaluacion();
        InitBuffer();
        return res;
    }
}
