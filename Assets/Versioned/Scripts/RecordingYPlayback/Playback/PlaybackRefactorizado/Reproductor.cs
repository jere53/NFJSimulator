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
    
    public delegate void MostrarInfraccion(DatosEvaluacion datosEvaluacion);
    public event MostrarInfraccion OnMostrarInfraccion;
    
    
    private Coroutine coroutineGrabacion;
    private Coroutine coroutineClima;

    private int currentFrame = 0;

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
        currentFrame = 0;
        while(existenFrames)
        {
            existenFrames = _estructuraGrabacion.SiguienteIntervalo();
            OnPlayIntervalo?.Invoke();
            try
            {
                OnMostrarInfraccion?.Invoke(_estructuraGrabacion.evals[currentFrame]);
            }
            catch (ArgumentException e)
            {
                Debug.LogWarning(e);
            }

            currentFrame++;
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
