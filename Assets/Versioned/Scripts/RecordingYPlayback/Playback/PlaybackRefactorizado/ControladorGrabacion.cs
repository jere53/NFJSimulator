using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ControladorGrabacion : MonoBehaviour
{
    
    [SerializeField] private EstructuraGrabacion _estructuraGrabacion;
    public InicializadorGrabaciones inicializadorGrabaciones;
    public Reproductor _reproductor;
    
    //paths de los archivos
    public string pathHeader;
    public string pathGrabacion;
    public string pathClima;
    public string pathToEvals;

    //public float velocidadReproduccion = 1;
    
    private int recordingFps;
    private int cantidadIntervalos;
    
    private int climaYToDFPS;
    private int cantidadIntervalosClimaYToD;
    
    private bool _isSlowMotion = false;
    
    public GameObject PlaybackControls;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInterpolar();
            Debug.Log("Interpolacion Cambiada");
        }
    }

    public void ComenzarPlayback(float velocidadReproduccion, bool interpolar)
    {
        AbirGrabacion();
        ReproducirGrabacion(velocidadReproduccion, interpolar);
        PlaybackControls.gameObject.SetActive(true);
    }
    
    private void AbirGrabacion()
    {
        inicializadorGrabaciones.InicializarGrabacion(pathHeader, _estructuraGrabacion, _reproductor);
        inicializadorGrabaciones.CargarFrames(pathGrabacion, pathClima, _estructuraGrabacion);
        inicializadorGrabaciones.CargarEvals(pathToEvals, _estructuraGrabacion);
        
        recordingFps = inicializadorGrabaciones.recordingFps;
        cantidadIntervalos = inicializadorGrabaciones.cantidadIntervalos;
        climaYToDFPS = inicializadorGrabaciones.climaYToDFPS;
        cantidadIntervalosClimaYToD = inicializadorGrabaciones.cantidadIntervalosClimaYToD;
    }

    private void ReproducirGrabacion(float velocidadReproduccion, bool interpolar)
    {
        SetearVelocidad(velocidadReproduccion);
        _reproductor.interpolar = interpolar;
        _reproductor.Reproducir();
    }
    
    public void SetearVelocidad(float velocidadReproduccion)
    {
        float deltaIntervalosRecording = 1/(recordingFps * velocidadReproduccion);
        float deltaIntervalosClima = 1/(climaYToDFPS * velocidadReproduccion);
        _reproductor.deltaIntervalosRecording = deltaIntervalosRecording;
        _reproductor.deltaIntervalosClima = deltaIntervalosClima;

        // Debug.Log("FPS: " + recordingFps + "    Velocidad Rep: " + velocidadReproduccion + "Delta Intervalo: " + deltaIntervalosRecording);
    }

    private void ToggleInterpolar()
    {
        _reproductor.interpolar = !_reproductor.interpolar;
    }

    public void ToggleSlowMotion()
    {
        SetearVelocidad(_isSlowMotion ? 1 : 0.5f);
        _isSlowMotion = !_isSlowMotion;
    }
}
