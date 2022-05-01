
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

    public TMPro.TMP_InputField _maxMinutos;
    public TMPro.TMP_InputField _maxSegundos;
    public TMPro.TMP_InputField _maxCombustible;
    public TMPro.TMP_InputField _maxVelocidad;

    public TMP_Dropdown _seleccionEjercicio;

    public LocalSimulationController simController;

    public void ComenzarEjercicio()
    {
        simController.ComenzarEjercicio((_seleccionEjercicio.value - 1), _toggleTiempo.isOn, int.Parse(_maxMinutos.text),
            int.Parse(_maxSegundos.text), _toggleConsumo.isOn, int.Parse(_maxCombustible.text),
            _toggleVelocidad.isOn, int.Parse(_maxVelocidad.text), _toggleSemaforos.isOn, _toggleAccidentes.isOn);
    }

}
