using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using VehiclePhysics;

public class CriterioNafta : MonoBehaviour, ICriterio
{
    public float objetivoLitrosConsumidos;
    private float _litrosConsumidos;
    private float _tiempoActual;
    
    public float granularidad = 4; //tiempo minimo en segundos desde la ultima notificacion para hacer otra notificacion 
    private float _tiemerGranularidad;

    private List<Tuple<float, float>> _consumoInstantaneoList;
    private List<Tuple<float, float>> _consumoAccum;
    private VehicleBase _vehicle;

    private bool isRecording;
    private CapturadorErrores _capturadorErrores;

    private void Awake()
    {
        _vehicle = GetComponent<VehicleBase>();

        if (!_vehicle)
        {
            Debug.LogError("No se encontro un VPVehicleBase en el G.O. correspondiente al vehiculo del trainee");
        }
    }

    // Update is called once per frame
    void Update()
    {
        _tiempoActual += Time.deltaTime;
        //consumo de combustible actual en gramos/segundo
        float consumoInstantaneo = _vehicle.data.Get(Channel.Vehicle, VehicleData.EngineFuelRate) / 1000.0f;
        if (consumoInstantaneo < 0)
        {
            consumoInstantaneo = 0; //en el primer frame, el consumo instantaneo es negativo por alguna razon. esto es un workaround
        }

        consumoInstantaneo /= 740; //para pasarlo a litros. El consumo esta en gramos/segundo, un litro de nafta son
        //approx 740 gramos, por lo tanto dividimos por 740
        
        if (consumoInstantaneo > 0)
            _litrosConsumidos += consumoInstantaneo * Time.deltaTime;
        
        if (_tiemerGranularidad > 0f)
        {
            _tiemerGranularidad -= Time.deltaTime;

            return;
        }

        _consumoInstantaneoList.Add( new Tuple<float, float>(_tiempoActual, consumoInstantaneo ));
        _consumoAccum.Add(new Tuple<float, float>(_tiempoActual, _litrosConsumidos));
        
        _tiemerGranularidad = granularidad;

        if (isRecording)
        {
            if (_litrosConsumidos > objetivoLitrosConsumidos)
            {
                _capturadorErrores.AddCapturaNafta(_litrosConsumidos);
                DisableRecording(); //solo se captura el momento del exceso
            }
        }
    }

    public void ObtenerDatosEvaluacion(ref DatosEvaluacion datosEvaluacion)
    {
        
        datosEvaluacion.DatosCriterioNafta = new DatosCriterioNafta();
        datosEvaluacion.DatosCriterioNafta.ConsumoInstantaneo = _consumoInstantaneoList;
        datosEvaluacion.DatosCriterioNafta.LitrosConsumidos = _litrosConsumidos;
        datosEvaluacion.DatosCriterioNafta.ObjetivoListrosConsumidos = objetivoLitrosConsumidos;
        datosEvaluacion.DatosCriterioNafta.ConstumoAccum = _consumoAccum;
    }

    public void ComenzarEvaluacion()
    {
        _consumoInstantaneoList = new List<Tuple<float, float>>();
        _consumoAccum = new List<Tuple<float, float>>();
        _tiempoActual = 0;
        _litrosConsumidos = 0;
        enabled = true;
    }

    public void ConcluirEvaluacion()
    {
        enabled = false;
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
