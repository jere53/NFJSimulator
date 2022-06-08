using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehiclePhysics;

[Serializable]
public class GroundWeatherInteraction : IWeatherObserver
{
    private GroundMaterial groundMaterial;
    private WeatherController weatherController;
    private string currentWeather = "sunny";
    public GripAndDragFactors[] gripAndDragFactorsEditor = new GripAndDragFactors[0];
    public Dictionary<string, GripAndDragFactors> gripAndDragFactors = new Dictionary<string, GripAndDragFactors>();

    public void SetGroundMaterial(GroundMaterial gm) {
        groundMaterial = gm;
    }

    public void SetWeatherController(WeatherController weatherController) {
        this.weatherController = weatherController;
    }

    public void BuildGADFDictionary() {
        foreach (var item in gripAndDragFactorsEditor)
        {
            gripAndDragFactors.Add(item.weatherName, item);
        }
    }

    public void UpdateWeather() 
    {

        //observer que actualiza los valores que retornara a las ruedas segun el clima
        //lee el clima actual de la clase que implemente el control del clima
        currentWeather = weatherController.GetCurrentWeatherPreset();
        //Debug.Log("GWI asociado al " + groundMaterial.physicMaterial.name + " cambio clima a " + currentWeather);
        ModifyGroundMaterialProperties();
    }

    public virtual void ModifyGroundMaterialProperties()
    {
        if (!gripAndDragFactors.ContainsKey(currentWeather)) {
            //Debug.Log("no tiene la clave");
            groundMaterial.drag = 0f;
            groundMaterial.grip = 1f;
            return;
        }
        groundMaterial.drag = gripAndDragFactors[currentWeather].dragFactor;
        groundMaterial.grip = gripAndDragFactors[currentWeather].gripFactor;
    }

    void OnDisable()
    {
        //Si el GWI no se va a usar mas, tiene que dejar de atender los eventos del weatherController.
        weatherController.Detach(this);
    }
}

[Serializable]
public struct GripAndDragFactors{
    public GripAndDragFactors(string weatherName, float gripFactor, float dragFactor){
        this.weatherName = weatherName;
        this.gripFactor = gripFactor;
        this.dragFactor = dragFactor;
    }

    public string weatherName;
    public float gripFactor;
    public float dragFactor;
}