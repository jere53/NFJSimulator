using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class WeatherController : MonoBehaviour
{
    public Volume volume;
    private VolumeProfile _volumeProfile;
    public GameObject rainParticleSystem;
    public string currentWeatherPreset = "sunny";
    public List<IWeatherObserver> observers = new List<IWeatherObserver>();
    private VolumetricClouds _volumetricClouds;

    private void Awake()
    {
        _volumeProfile = volume.sharedProfile; //conseguimos la referencia al perfil del volumen del cielo.
        if (!_volumeProfile.TryGet<VolumetricClouds>(out _volumetricClouds))
        {
            Debug.Log("No se encontro VolumetricClouds override en el perfil de volumen");
        }
    }

    //public Volume skyAndFogVolume;
    void NotifyWeatherChange() {
        foreach (var observer in observers)
        {
            observer.UpdateWeather();
        }
    }

    public void Attach(IWeatherObserver observer) {
        observers.Add(observer);
    }

    public void Detach(IWeatherObserver observer) {
        observers.Remove(observer);
    }

    public string GetCurrentWeatherPreset() {
        return currentWeatherPreset;
    }

    public void SunnyPreset() {
        //settear efectos
        _volumetricClouds.cloudPreset.overrideState = true; //que use el valor seteado aqui
        _volumetricClouds.cloudPreset.value = VolumetricClouds.CloudPresets.Sparse; //pocas nubes
        rainParticleSystem.SetActive(false);
        //
        currentWeatherPreset = "sunny";
        NotifyWeatherChange();
    }

    public void RainPreset() {
        if(rainParticleSystem == null) return;
        
        //efectos
        rainParticleSystem.SetActive(true);
        _volumetricClouds.cloudPreset.overrideState = true;
        _volumetricClouds.cloudPreset.value = VolumetricClouds.CloudPresets.Overcast;
        //
        currentWeatherPreset = "rain";
        NotifyWeatherChange();
    }

    public void CloudyPreset() {
        //efectos
        _volumetricClouds.cloudPreset.overrideState = true;
        _volumetricClouds.cloudPreset.value = VolumetricClouds.CloudPresets.Stormy;
        rainParticleSystem.SetActive(false);
        //
        currentWeatherPreset = "cloudy";
        NotifyWeatherChange();
    }

    // private void Update() {
    //     if (Input.GetMouseButtonDown(0)) {
    //         RainPreset();
    //     }
    //
    //     if (Input.GetMouseButtonDown(1))
    //     {
    //         SunnyPreset();
    //     }
    //
    //     if (Input.GetMouseButtonDown(2))
    //     {
    //         CloudyPreset();
    //     }
    // }
}
