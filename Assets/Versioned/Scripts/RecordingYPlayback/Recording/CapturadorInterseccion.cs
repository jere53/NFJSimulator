using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public struct EstadoInicialInterseccion
{
    public float segundosActivada;
    public float tiempoCadaSemaforoEnVerde;
}

public class CapturadorInterseccion : MonoBehaviour
{
    public int id;
    
    private bool _cronometroActivado;

    private float _tiempoActual;

    public void ComenzarARegistrar()
    {
        _cronometroActivado = true;
        Recorder.instancia.OnCapture += RegistrarEstadoInicial;
    }

    public void RegistrarEstadoInicial()
    {
        EstadoInicialInterseccion estadoInicialInterseccion = new EstadoInicialInterseccion
        {
            tiempoCadaSemaforoEnVerde = GetComponent<Interseccion>().tiempoCadaSemaforoEnVerde,
            segundosActivada = _tiempoActual
        };
        
        Recorder.instancia.estadosInicialesIntersecciones.TryAdd(id, estadoInicialInterseccion);

        _cronometroActivado = false;
        Recorder.instancia.OnCapture -= RegistrarEstadoInicial;
    }
    
    private void OnDisable()
    {
        _cronometroActivado = false;
        Recorder.instancia.OnCapture -= RegistrarEstadoInicial;
    }

    void Update()
    {
        if (_cronometroActivado)
        {
            _tiempoActual += Time.deltaTime;
        }
    }
}
*/