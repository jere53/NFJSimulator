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
    private bool pause = false;

    public float tiempoIntervalo;
    
    
    [NonSerialized]public WeatherController weatherController;
    [NonSerialized]public DayNightCicle dayNightCicle;
    
    public delegate void ActualizarPosicion();
    public event ActualizarPosicion OnPlayIntervalo;
    
    public delegate void MostrarInfraccion(DatosEvaluacion datosEvaluacion);
    public event MostrarInfraccion OnMostrarInfraccion;
    
    public event ActualizarPosicion OnPauseIntervalo;
    
    private Coroutine coroutineGrabacion;
    private Coroutine coroutineClima;

    private int currentFrame = 0;
    
    ControladorGrabacion ControladorGrabacion;
    
    private bool _rewind = false;

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

    public void Pausar()
    {
        pause = !pause;
        OnPauseIntervalo?.Invoke();
    }

    public void ToggleRewind()
    {
        if (pause)
        {
            Pausar();
        }
        _rewind = !_rewind;
    }
    
    public void CambiarVelocidad(float velocidadReproduccion)
    {
        ControladorGrabacion.SetearVelocidad(velocidadReproduccion);
    }
    
    public void ToggleSlowMotion()
    {
        if (pause)
        {
            Pausar();
        }
        ControladorGrabacion.ToggleSlowMotion();
    }

    private void Start()
    {
        ControladorGrabacion = GetComponent<ControladorGrabacion>();
    }

    public IEnumerator Play()
    {
        bool existenFrames = true;
        currentFrame = 0;
        while(existenFrames)
        {
            if (pause == false) 
            {
                existenFrames = _estructuraGrabacion.SiguienteIntervalo(_rewind);
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
                
            }
           yield return new WaitForSeconds(deltaIntervalosRecording);
        }
    }

    public IEnumerator PlayClimaAndToD()
    {
        bool existenFrames = true;
        while (existenFrames)
        {
            //Se decidio no hacer pausa del estado del clima aunque el resto de las cosas este pausado. 
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
