using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotonConfirmar : MonoBehaviour
{
    public Slider _sliderVehiculos;
    public Slider _sliderPeatones;
    public Slider _sliderHoraDelDia;
    public Slider _sliderVelocidadDia;
    public TMPro.TMP_Dropdown seleccionClima;

    public LocalSimulationController simulationController;

    public void Confirmar()
    {
        
        switch (seleccionClima.value)
        {
            case 0 or 1:
                simulationController.SetWeather("sunny");
                break;
            case 2: 
                simulationController.SetWeather("rainy");
                break;
            case 3: 
                simulationController.SetWeather("cloudy");
                break;
        }
        
        simulationController.SetPeatones((int)_sliderPeatones.value);
        simulationController.SetVehiculos((int)_sliderVehiculos.value);
        simulationController.SetToD(_sliderHoraDelDia.value, _sliderVelocidadDia.value);
    }


}
