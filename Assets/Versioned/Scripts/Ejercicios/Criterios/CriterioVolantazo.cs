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

    public float granularidad = 4f; //para que no capture la misma infraccion muchas veces, un "cooldown" hasta que capture la proxima

    private float _tiempoActual;

    private float _timer;

    private float _tiempoHastaCaptura;

    private List<Tuple<float, float>> _yawEnTiempo;

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

        if (_timer < -10000f)
        {
            _timer = -1f; //para que no se rompa si pasa mucho tiempo sin resettear
        }

        if (_timer > 0f)
        {
            _timer -= Time.fixedDeltaTime;
            return;
        }
        
        if (Mathf.Abs(_vehicle.yawVelocity) >= doblajeSeguroMaximo)
        {
            _infracciones.Add(new Tuple<float, float, float>(_tiempoActual, _vehicle.yawVelocity, doblajeSeguroMaximo));
            _yawEnTiempo.Add(new Tuple<float, float>(_tiempoActual, _vehicle.yawVelocity));
            //Debug.Log("volantazo! " + _vehicle.yawVelocity + "rads/s de doblaje, max seguro es: " + doblajeSeguroMaximo + "rads/s");
            _timer = granularidad;
            _tiempoHastaCaptura = granularidad;
            return;
        }

        if (_tiempoHastaCaptura > 0f)
        {
            _tiempoHastaCaptura -= Time.fixedDeltaTime;
            return;
        }
        
        _yawEnTiempo.Add(new Tuple<float, float>(_tiempoActual, _vehicle.yawVelocity));
        
        _tiempoHastaCaptura = granularidad;

    }

    public void ObtenerDatosEvaluacion(ref DatosEvaluacion datos)
    {
        datos.DatosCriterioVolantazos = new DatosCriterioVolantazos();
        datos.DatosCriterioVolantazos.Infracciones = _infracciones;
        datos.DatosCriterioVolantazos.YawEnTiempo = _yawEnTiempo;
    }

    public void ComenzarEvaluacion()
    {
        _yawEnTiempo = new List<Tuple<float, float>>();
        enabled = true;
        _tiempoHastaCaptura = 0;
        _timer = 0;
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
