
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BotonComenzarEjercicio : MonoBehaviour
{
    public Toggle _toggleTiempo;
    public Toggle _toggleConsumo;
    public Toggle _toggleVelocidad;
    public Toggle _toggleAccidentes;
    public Toggle _toggleSemaforos;
    public Toggle _toggleVolantazos;
    public Toggle _toggleAceleracion;
    public Toggle _toggleRPM;

    public TMP_InputField _maxMinutos;
    public TMP_InputField _maxSegundos;
    
    public TMP_InputField _maxCombustible;
    
    public TMP_InputField _maxVelocidad;
    
    public TMP_InputField _maximoDoblaje;

    public TMP_InputField _maxAceleracion;

    public TMP_InputField _minRPM;
    public TMP_InputField _maxRPM;

    public TMP_Dropdown _seleccionEjercicio;

    public LocalSimulationController simController;

    public void ComenzarEjercicio()
    {
        SeleccionCriterios seleccionCriterios = new SeleccionCriterios();

        seleccionCriterios.Tiempo.Evaluar = _toggleTiempo.isOn;
        seleccionCriterios.Tiempo.MaxMinutos = int.Parse(_maxMinutos.text);
        seleccionCriterios.Tiempo.MaxSegundos = int.Parse(_maxSegundos.text);

        seleccionCriterios.Combustible.Evaluar = _toggleConsumo.isOn;
        seleccionCriterios.Combustible.MaxLitros = int.Parse(_maxCombustible.text);

        seleccionCriterios.Velocidad.Evaluar = _toggleVelocidad.isOn;
        seleccionCriterios.Velocidad.MaxVelocidad = int.Parse(_maxVelocidad.text);

        seleccionCriterios.Semaforos.Evaluar = _toggleSemaforos.isOn;

        seleccionCriterios.Accidentes.Evaluar = _toggleAccidentes.isOn;

        seleccionCriterios.Volantazos.Evaluar = _toggleVolantazos.isOn;
        seleccionCriterios.Volantazos.MaxDoblaje = float.Parse(_maximoDoblaje.text);

        seleccionCriterios.Aceleracion.Evaluar = _toggleAceleracion.isOn;
        seleccionCriterios.Aceleracion.MaxAceleracion = float.Parse(_maxAceleracion.text);

        seleccionCriterios.Rpm.Evaluar = _toggleRPM.isOn;
        seleccionCriterios.Rpm.MaxRpm = int.Parse(_maxRPM.text);
        seleccionCriterios.Rpm.MinRpm = int.Parse(_minRPM.text);
        
        simController.ComenzarEvaluacion(seleccionCriterios);
    }

}
