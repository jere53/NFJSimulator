
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
        simController.ComenzarEjercicio((_seleccionEjercicio.value - 1),
            _toggleTiempo.isOn, int.Parse(_maxMinutos.text),int.Parse(_maxSegundos.text), 
            _toggleConsumo.isOn, int.Parse(_maxCombustible.text), 
            _toggleVelocidad.isOn, int.Parse(_maxVelocidad.text),
            _toggleSemaforos.isOn, 
            _toggleAccidentes.isOn,
            _toggleVolantazos.isOn, float.Parse(_maximoDoblaje.text), 
            _toggleAceleracion.isOn, float.Parse(_maxAceleracion.text),
            _toggleRPM.isOn, int.Parse(_minRPM.text), int.Parse(_maxRPM.text));
    }

}
