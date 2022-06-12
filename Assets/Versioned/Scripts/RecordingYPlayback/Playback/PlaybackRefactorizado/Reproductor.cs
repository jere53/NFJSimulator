using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Reproductor : MonoBehaviour
{

    [DoNotSerialize] public EstructuraGrabacion _estructuraGrabacion;

    public bool interpolar;
    
    public float deltaIntervalosRecording;
    public float deltaIntervalosClima;
    private float timeElapsedRecording = 0;
    private float timeElapsedClima = 0;

    public float tiempoIntervalo;
    
    
    [NonSerialized]public WeatherController weatherController;
    [NonSerialized]public DayNightCicle dayNightCicle;
    
    public delegate void ActualizarPosicion();
    public event ActualizarPosicion OnPlayIntervalo;
    
    private Coroutine coroutineGrabacion;
    private Coroutine coroutineClima;


    // Update is called once per frame
    // void Update()
    // {
    //     if (play)
    //     {
    //         //Cuando pasa el tiempo que tiene que pasar entre cada intervalo de la grabacion, cambio de frame
    //         if (timeElapsedRecording < deltaIntervalosRecording)
    //         {
    //             timeElapsedRecording += Time.deltaTime;
    //         }
    //         else
    //         {
    //             timeElapsedRecording = 0;
    //             _estructuraGrabacion.SiguienteIntervalo();
    //             OnPlayIntervalo?.Invoke();
    //         }
    //         
    //         //Lo mismo pero con el clima
    //         if (timeElapsedClima < deltaIntervalosClima)
    //         {
    //             //valueToLerp = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
    //             timeElapsedClima += Time.deltaTime;
    //         }
    //         else
    //         {
    //             timeElapsedClima = 0;
    //             _estructuraGrabacion.PlayIntervaloClimaToD();
    //         }
    //     }
    // }

    public void Reproducir()
    {
        coroutineGrabacion = StartCoroutine(Play());
        coroutineClima = StartCoroutine(PlayClimaAndToD());
    }

    public void Detener()
    {
        StopCoroutine(coroutineGrabacion);
        StopCoroutine(coroutineClima);
    }
    
    public IEnumerator Play()
    {
        bool existenFrames = true;
        while(existenFrames)
        {
            existenFrames = _estructuraGrabacion.SiguienteIntervalo();
            OnPlayIntervalo?.Invoke();
            yield return new WaitForSeconds(deltaIntervalosRecording);
        }
    }

    public IEnumerator PlayClimaAndToD()
    {
        bool existenFrames = true;
        while (existenFrames)
        {
            existenFrames = _estructuraGrabacion.SiguienteIntervaloClimaToD();
            switch (_estructuraGrabacion.intervaloClimaToDActual.clima)
            {
                case 0:
                    weatherController.SunnyPreset();
                    break;
                case 1:
                    weatherController.RainPreset();
                    break;
                case 2:
                    weatherController.CloudyPreset();
                    break;
            }

            dayNightCicle.orbitSpeed = _estructuraGrabacion.intervaloClimaToDActual.velocidadOrbita;
            dayNightCicle.timeOfDay = _estructuraGrabacion.intervaloClimaToDActual.hora;
            yield return new WaitForSeconds(deltaIntervalosClima);
        }
    }


    
}
