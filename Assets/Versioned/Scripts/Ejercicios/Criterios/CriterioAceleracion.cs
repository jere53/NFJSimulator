using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehiclePhysics;

public class CriterioAceleracion : MonoBehaviour, ICriterio
{

    private VPVehicleToolkit _vehicle;
    private float _tiempoActual;

    private bool _evaluando = false; //enable/disable no afecta FixedUpdate

    private List<Tuple<float, float, float, float, float>> _infracciones = new List<Tuple<float, float, float, float, float>>(); 
    //tiempo, Along, Alat, Avert, Amax
    private List<Tuple<float, float, float, float, float>> _aceleracionesEnTiempo = new List<Tuple<float, float, float, float, float>>(); 

    public float granularidad = 4; //tiempo minimo en segundos desde la ultima notificacion para hacer otra notificacion 
    private float _tiemerGranularidad;

    public float maximaAceleracion; //positivo para aceleradas, negativo para frenadas.

    private void Awake()
    {
        _vehicle = GetComponent<VPVehicleToolkit>();
        if (!_vehicle)
        {
            Debug.LogError("No se encontro un VPVehicleToolkit en el G.O. correspondiente al vehiculo del trainee");
        }
    }

    private void FixedUpdate()
    {
        if (!_evaluando) return;
        
        _tiempoActual += Time.fixedDeltaTime;

        if (_tiemerGranularidad > 0f)
        {
            _tiemerGranularidad -= Time.fixedDeltaTime;

            return;
        }

        if (Mathf.Abs(_vehicle.longitudinalG) > maximaAceleracion ||
            Mathf.Abs(_vehicle.lateralG) > maximaAceleracion || 
            Mathf.Abs(_vehicle.verticalG) > maximaAceleracion)
        {
            _infracciones.Add(new Tuple<float, float, float, float, float>(_tiempoActual, _vehicle.longitudinalG, 
                _vehicle.lateralG, _vehicle.verticalG, maximaAceleracion));
            
            _aceleracionesEnTiempo.Add(new Tuple<float, float, float, float, float>(_tiempoActual, _vehicle.longitudinalG, 
                _vehicle.lateralG, _vehicle.verticalG, maximaAceleracion));
        
            _tiemerGranularidad = granularidad;
            
            return;

            //Debug.Log("exceso en la aceleracion! detectados" + _vehicle.longitudinalG + "; " + _vehicle.lateralG +";" + _vehicle.verticalG + " max aceleracion = " + maximaAceleracion);
        }
        
        _aceleracionesEnTiempo.Add(new Tuple<float, float, float, float, float>(_tiempoActual, _vehicle.longitudinalG, 
            _vehicle.lateralG, _vehicle.verticalG, maximaAceleracion));
        
        _tiemerGranularidad = granularidad;
    }

    public void ObtenerDatosEvaluacion(ref DatosEvaluacion datos)
    {
        datos.DatosCriterioAceleracion = new DatosCriterioAceleracion();
        datos.DatosCriterioAceleracion.AceleracionesEnTiempo = _aceleracionesEnTiempo;
        datos.DatosCriterioAceleracion.Infracciones = _infracciones;
    }

    public void ComenzarEvaluacion()
    {
        _tiempoActual = 0;
        _tiemerGranularidad = 0;
        _infracciones.Clear();
        _evaluando = true;
    }

    public void ConcluirEvaluacion()
    {
        _evaluando = false;
    }

    public void Remover()
    {
        Destroy(this);
    }
}
