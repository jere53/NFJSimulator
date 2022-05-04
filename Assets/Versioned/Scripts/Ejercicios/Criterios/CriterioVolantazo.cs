using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using VehiclePhysics;

public class CriterioVolantazo : MonoBehaviour, ICriterio
{

    private VPVehicleToolkit _vehicle;

    public float doblajeSeguroMaximo; //maxima velocidad de doblaje, radianes por segundo

    public float granularidad = 1; //para que no capture la misma infraccion muchas veces, un "cooldown" hasta que capture la proxima

    private float _tiempoActual;

    private float _timer;

    //private int _ultimaInputSteering = 0;

    private List<Tuple<float, float, float>> _infracciones = new List<Tuple<float, float, float>>();
    //_tiempoActual, doblajeRealizado, doblajeMaximo

    private void Awake()
    {
        _vehicle = GetComponent<VPVehicleToolkit>();

        if (!_vehicle)
        {
            Debug.LogError("No se encontro un VPVehicleToolkit en el vehiculo del trainee");
        }
    }


    private void FixedUpdate()
    {
        _tiempoActual += Time.fixedDeltaTime;

        if (_timer > 0f)
        {
            _timer -= Time.fixedDeltaTime;
            return;
        }
        
        if (Mathf.Abs(_vehicle.yawVelocity) >= doblajeSeguroMaximo)
        {
            _infracciones.Add(new Tuple<float, float, float>(_tiempoActual, _vehicle.yawVelocity, doblajeSeguroMaximo));
            //Debug.Log("volantazo! " + _vehicle.yawVelocity + "rads/s de doblaje, max seguro es: " + doblajeSeguroMaximo + "rads/s");
        }
        
        _timer = granularidad;
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
         //DEPRECATED: Idea original de usar la input para medir volantazos.
        _tiempoActual += Time.deltaTime;

        if (_timer > 0f)
        {
            _timer -= Time.deltaTime;
            return;
        }
        
        int inputSteering = _vehicle.data.Get(Channel.Input, InputData.Steer) / 100; //el DB va de -10000 a +10000, divimos por 100 para que nos quede de
        //-100 a 100, y usarlo como porcentaje

        int doblajeRealizado = Mathf.Abs(inputSteering - _ultimaInputSteering);
        
        if (doblajeRealizado >= doblajeSeguroMaximo)
        {
            _infracciones.Add(new Tuple<float, int, int, int>(_tiempoActual, doblajeRealizado, doblajeSeguroMaximo, granularidad));
            Debug.Log("volantazo! " + doblajeRealizado + " de doblaje en " + granularidad + "max seguro era: " + doblajeSeguroMaximo );
        }
        
        _timer = granularidad;
        
        _ultimaInputSteering = _vehicle.data.Get(Channel.Input, InputData.Steer) / 100;
        */

    }

    public void ObtenerDatosEvaluacion(ref DatosEvaluacion datos)
    {
        datos.DatosCriterioVolantazo = _infracciones;
    }

    public void ComenzarEvaluacion()
    {
        enabled = true;
        _timer = granularidad;
        _tiempoActual = 0;
        _infracciones.Clear();
    }

    public void ConcluirEvaluacion()
    {
        enabled = false;
    }

    public void Remover()
    {
        Destroy(this);
    }
}
