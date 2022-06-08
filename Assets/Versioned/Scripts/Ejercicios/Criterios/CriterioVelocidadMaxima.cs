using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehiclePhysics;

public class CriterioVelocidadMaxima : MonoBehaviour, ICriterio
{

    public float velocidadMaximaKMh;

    private VehicleBase _vehicle;
    
    private List<Tuple<float, float, bool>> _infracciones = new List<Tuple<float, float, bool>>();

    private List<Tuple<float, float>> _velocidadesEnTiempo = new List<Tuple<float, float>>();
    
    private float _tiempoActual;

    private bool _enInfraccion;

    public float granularidad = 4; //cada cuantos segundos registrar la velocidad, si esta en exceso. 

    private float _tiempoHastaMedida = 0f;

    private float _timerGranularidad;

    private bool isRecording;
    private CapturadorErrores _capturadorErrores;


    private void Awake()
    {
        _vehicle = GetComponent<VehicleBase>();

        if (!_vehicle)
        {
            Debug.LogError("No se encontro un VPVehicleBase en el G.O. correspondiente al Trainee");
        }
    }

    // Start is called before the first frame update
    private void Update()
    {
        _tiempoActual += Time.deltaTime;
        
        float velocidadActual =
            (_vehicle.data.Get(Channel.Vehicle, VehicleData.Speed) / 1000f) * 3.6f; //para llevarlo a KM/h 
        
        //Para no guardar muchos datos innecesarios, tenemos el flag enInfraccion. Cuando se detecta un exceso de velocidad,
        //se marca el comienzo de la infraccion y se registran las velocidades en exceso cada approx. <granularidad> segundos.
        //Cuando se vuelva a los confines de la velocidadMaxima, se resettea el flag enInfraccion.
        
        if (velocidadActual > velocidadMaximaKMh)
        {
            _enInfraccion = true;
            if (_tiempoHastaMedida <= 0f)
            {
                _infracciones.Add(new Tuple<float, float, bool >(_tiempoActual, velocidadActual, true));
                _tiempoHastaMedida = granularidad;
                
                _velocidadesEnTiempo.Add(new Tuple<float, float>(_tiempoActual, velocidadActual));

                _timerGranularidad = granularidad;

                if (isRecording)
                {
                    _capturadorErrores.AddCapturaVelocidad(new Tuple<float, float, bool >(_tiempoActual, velocidadActual, true));
                }
            }
            else
            {
                _tiempoHastaMedida -= Time.deltaTime;
            }
        }
        else
        {
            if (_enInfraccion) //significa que se estaba excediendo pero ya paro, entonces se agrega a la lista el "final" de la excesion
            {
                _infracciones.Add(new Tuple<float, float, bool>(_tiempoActual, velocidadActual, false));

                if (isRecording)
                {
                    _capturadorErrores.AddCapturaVelocidad(
                        new Tuple<float, float, bool>(_tiempoActual, velocidadActual, false));
                }
                
            }
            _enInfraccion = false;
            _tiempoHastaMedida = 0f;
        }
        
        if (_timerGranularidad > 0f)
        {
            _timerGranularidad -= Time.deltaTime;

            return;
        }
        
        _velocidadesEnTiempo.Add(new Tuple<float, float>(_tiempoActual, velocidadActual));

        _timerGranularidad = granularidad;
    }
    

    public void ObtenerDatosEvaluacion(ref DatosEvaluacion resultado)
    {
        resultado.DatosCriterioVelocidad = new DatosCriterioVelocidad();
        resultado.DatosCriterioVelocidad.Infracciones= _infracciones;
        resultado.DatosCriterioVelocidad.VelocidadEnTiempo = _velocidadesEnTiempo;
        resultado.DatosCriterioVelocidad.VelocidadMaxima = velocidadMaximaKMh;
    }

    public void ComenzarEvaluacion()
    {
        _velocidadesEnTiempo.Clear();
        _infracciones.Clear();
        enabled = true;
        _tiempoActual = 0;
        _enInfraccion = false;
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
