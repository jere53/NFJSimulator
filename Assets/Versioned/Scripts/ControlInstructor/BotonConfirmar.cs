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

    public SimulationController simulationController;

    public void Confirmar()
    {
        
        switch (seleccionClima.value)
        {
            case 0 or 1:
                simulationController.SetWeatherServerRpc("sunny");
                break;
            case 2: 
                simulationController.SetWeatherServerRpc("rainy");
                break;
            case 3: 
                simulationController.SetWeatherServerRpc("cloudy");
                break;
        }
        
        simulationController.SetPeatonesServerRpc((int)_sliderPeatones.value);
        simulationController.SetVehiculosServerRpc((int)_sliderVehiculos.value);
        simulationController.SetToDServerRpc(_sliderHoraDelDia.value, _sliderVelocidadDia.value);
    }


}
