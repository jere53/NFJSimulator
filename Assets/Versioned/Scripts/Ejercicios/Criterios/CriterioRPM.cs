using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehiclePhysics;

public class CriterioRPM : MonoBehaviour, ICriterio
{
    public int minimoRpm;
    public int maximoRpm;
    
    private List<Tuple<float, int, int, int, bool>> _infracciones;
    
    private float _tiempoActual;

    private bool _enInfraccion;

    public float granularidad = 4; //cada cuantos segundos registrar las RPMs, si estan en falta. 

    private float _tiempoHastaMedida = 0f;

    private float _timerGranularidad;
    
    private VPVehicleController _vehicle;

    private List<Tuple<float, float>> _rpmEnTiempo;

    private bool isRecording;
    private CapturadorErrores _capturadorErrores;

    private void Awake()
    {

        _vehicle = GetComponent<VPVehicleController>();
        if (!_vehicle)
        { 
            Debug.LogError("No se encontro un VPVehicleBase en el G.O. correspondiente al Trainee");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        _tiempoActual += Time.deltaTime;
        
        int rpmActuales =
            // ReSharper disable once PossibleLossOfFraction
            //La resolucion del EngineRPM es 1000, es decir, 1 RPM = 1000 engine RPM. Si se pierde la fraccion el error es
            //a lo sumo 1 RPM, no es un numero significativo asi que hacemos division entera.
            (_vehicle.data.Get(Channel.Vehicle, VehicleData.EngineRpm) / 1000);
        
        
        //Para no guardar muchos datos innecesarios, tenemos el flag enInfraccion. Cuando se detecta un exceso de rpm,
        //se marca el comienzo de la infraccion y se registran las rpm en exceso cada approx. <granularidad> segundos.
        //Cuando se vuelva a los confines de las rpm maximas, se resettea el flag enInfraccion.
        
        if (rpmActuales < minimoRpm || rpmActuales > maximoRpm)
        {
            _enInfraccion = true;
            if (_tiempoHastaMedida <= 0f)
            {
                _infracciones.Add(new Tuple<float, int, int, int, bool >(_tiempoActual, minimoRpm, maximoRpm, rpmActuales, true));
                //Debug.Log("Infraccion RPM detectada! minimo/maximo = " + minimoRpm+";"+maximoRpm+" detectadas " + rpmActuales );
                _tiempoHastaMedida = granularidad;
                
                _rpmEnTiempo.Add(new Tuple<float, float>(_tiempoActual, rpmActuales));
                _timerGranularidad = granularidad;

                if (isRecording)
                {
                    _capturadorErrores.AddCapturaRpm(new Tuple<float, int, int, int, bool >(_tiempoActual, minimoRpm, maximoRpm, rpmActuales, true));
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
                _infracciones.Add(new Tuple<float, int, int, int, bool>(_tiempoActual, rpmActuales, minimoRpm, maximoRpm, false));

                if (isRecording)
                {
                    _capturadorErrores.AddCapturaRpm(new Tuple<float, int, int, int, bool>(_tiempoActual, minimoRpm,
                        maximoRpm, rpmActuales, false));
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

        _timerGranularidad = granularidad;
        
        _rpmEnTiempo.Add(new Tuple<float, float>(_tiempoActual, rpmActuales));

    }

    public void ObtenerDatosEvaluacion(ref DatosEvaluacion datos)
    {
        datos.DatosCriterioRpm = new DatosCriterioRpm();
        datos.DatosCriterioRpm.Infracciones = _infracciones;
        datos.DatosCriterioRpm.RpmEnTiempo = _rpmEnTiempo;
    }

    public void ComenzarEvaluacion()
    {
        _infracciones = new List<Tuple<float, int, int, int, bool>>();
        _rpmEnTiempo = new List<Tuple<float, float>>();
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
