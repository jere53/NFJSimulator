using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriterioEvitarAccidentes : MonoBehaviour, ICriterio
{
    private bool _evaluando;

    private float _tiempoActual;
    
    private List<float> _golpesAPeatones = new List<float>();
    private List<Tuple<float, int>> _golpesAVehiculos = new List<Tuple<float, int>>();

    private bool isRecording;
    private CapturadorErrores _capturadorErrores;

    private void Update()
    {
        _tiempoActual += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_evaluando) return;
        if (other.CompareTag("Peaton"))
        {
            _golpesAPeatones.Add(_tiempoActual);
            if (isRecording)
            {
                _capturadorErrores.AddCapturaAccidente(_tiempoActual);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!_evaluando) return;
        if (other.gameObject.CompareTag("Auto"))
        {
            _golpesAVehiculos.Add(new Tuple<float, int>(_tiempoActual, other.gameObject.GetInstanceID()));
            if (isRecording)
            {
                _capturadorErrores.AddCapturaAccidente(_tiempoActual);
            }
        }
    }

    public void ObtenerDatosEvaluacion(ref DatosEvaluacion datosEvaluacion)
    {
        datosEvaluacion.DatosCriterioEvitarAccidentes = new Tuple<List<float>, 
            List<Tuple<float, int>>>(_golpesAPeatones, _golpesAVehiculos);
        
    }

    public void ComenzarEvaluacion()
    {
        _golpesAPeatones.Clear();
        _golpesAVehiculos.Clear();
        _tiempoActual = 0;
        enabled = true;
        _evaluando = true;
    }

    public void ConcluirEvaluacion()
    {
        enabled = false;
        _evaluando = false;
    }

    public void Remover()
    {
        Destroy(this);
    }
    
    public void EnableRecording(CapturadorErrores capturadorErrores)
    {
        if(!capturadorErrores) return;
        
        isRecording = true;
    }

    public void DisableRecording()
    {
        isRecording = false;
    }

}
