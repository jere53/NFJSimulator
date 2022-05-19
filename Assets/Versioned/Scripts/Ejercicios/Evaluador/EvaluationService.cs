using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluationService
{
    private DatosEvaluacion _datosUltimaEvaluacion;
    public string pathCarpetaEvaluaciones = 
        System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Evaluaciones NFJSim");
    public void ComenzarEvaluacion(GameObject vehiculoTrainee, SeleccionCriterios criteriosEvaluacion)
    {
        
        //Si ya habia criterios por alguna razon, nos deshacemos de ellos para no "ensuciar" la nueva evaluacion
        var criteriosPrevios = vehiculoTrainee.GetComponents<ICriterio>();

        foreach (var c in criteriosPrevios)
        {
            Debug.LogWarning("Se comenzo una nueva evaluacion pero ya habia un criterio activo, se removio el" +
                             " criterio " + c + " que estaba activo.");
            c.Remover();
        }
        
        if (criteriosEvaluacion.Tiempo.Evaluar)
        {
            var cT = vehiculoTrainee.AddComponent<CriterioTiempo>();
            cT.minutosMaximosPermitidos = criteriosEvaluacion.Tiempo.MaxMinutos;
            cT.segundosMaximosPermitidos = criteriosEvaluacion.Tiempo.MaxSegundos;
        }

        if (criteriosEvaluacion.Combustible.Evaluar)
        {
            var cC = vehiculoTrainee.AddComponent<CriterioNafta>();
            cC.objetivoLitrosConsumidos = criteriosEvaluacion.Combustible.MaxLitros;
        }

        if (criteriosEvaluacion.Velocidad.Evaluar)
        {
            var cV = vehiculoTrainee.AddComponent<CriterioVelocidadMaxima>();
            cV.velocidadMaximaKMh = criteriosEvaluacion.Velocidad.MaxVelocidad;
        }

        if (criteriosEvaluacion.Semaforos.Evaluar)
        {
            vehiculoTrainee.AddComponent<CriterioRespetarSemaforos>();
        }
        
        if (criteriosEvaluacion.Accidentes.Evaluar)
        {
            vehiculoTrainee.AddComponent<CriterioEvitarAccidentes>();
        }

        if (criteriosEvaluacion.Volantazos.Evaluar)
        {
            var cV = vehiculoTrainee.AddComponent<CriterioVolantazo>();
            cV.doblajeSeguroMaximo = criteriosEvaluacion.Volantazos.MaxDoblaje;
        }

        if (criteriosEvaluacion.Aceleracion.Evaluar)
        {
            var cA = vehiculoTrainee.AddComponent<CriterioAceleracion>();
            cA.maximaAceleracion = criteriosEvaluacion.Aceleracion.MaxAceleracion;
        }

        if (criteriosEvaluacion.Rpm.Evaluar)
        {
            var cRpm = vehiculoTrainee.AddComponent<CriterioRPM>();
            cRpm.maximoRpm = criteriosEvaluacion.Rpm.MaxRpm;
            cRpm.minimoRpm = criteriosEvaluacion.Rpm.MinRpm;
        }

        var criterios = vehiculoTrainee.GetComponents<ICriterio>();
        foreach (var c in criterios)
        {
            c.ComenzarEvaluacion();
        }

        Debug.Log("Comenzo la evaluacion!");

    }

    public void ConcluirEvaluacion(GameObject vehiculoTrainee)
    {
        var criterios = vehiculoTrainee.GetComponents<ICriterio>();
        if (criterios.Length > 0)
        {
            Debug.Log("Concluyo la evaluacion!");
        }
        else
        {
            Debug.LogWarning("No habia ninguna evaluacion en progreso");
            return;
        }
        _datosUltimaEvaluacion = new DatosEvaluacion();
        foreach (var c in criterios)
        {
            c.ConcluirEvaluacion();
            c.ObtenerDatosEvaluacion(ref _datosUltimaEvaluacion);
            c.Remover();
        }
    }

    public void LogDatosEvaluacion()
    {
        _datosUltimaEvaluacion.Presentar();
    }

    public void AlmacenarDatosEvaluacion(string nombreArchivo)
    {
        if (_datosUltimaEvaluacion == null)
        {
            Debug.LogWarning("No hay Datos Evaluacion para guardar...Concluyo una evaluacion?");
            return;
        }

        try
        {
            System.IO.Directory.CreateDirectory(pathCarpetaEvaluaciones);
        }
        catch (Exception exception)
        {
            Debug.Log("Problema al crear el directorio donde guardar las evaluaciones.");
            Debug.LogWarning(exception);
        }

        _datosUltimaEvaluacion.GuardarEnDisco(pathCarpetaEvaluaciones+"\\"+nombreArchivo+".json");
    }

    public void CargarEvaluacionDesdeDisco(string nombreArchivo)
    {
        DatosEvaluacion.CargarDeDisco(pathCarpetaEvaluaciones+"\\"+ nombreArchivo+".json");
    }
    
    public void LogEvaluacionDesdeDisco(string nombreArchivo)
    {
        DatosEvaluacion.CargarDeDisco(pathCarpetaEvaluaciones+"\\"+ nombreArchivo+".json").Presentar();
    }
}

public struct SeleccionCriterios
{
    public struct SeleccionTiempo
    {
        public bool Evaluar;
        public int MaxMinutos;
        public int MaxSegundos;
    }

    public struct SeleccionCombustible
    {
        public bool Evaluar;
        public int MaxLitros;
    }
    
    public struct SeleccionVelocidad
    {
        public bool Evaluar;
        public int MaxVelocidad;
    }

    public struct SeleccionSemaforos
    {
        public bool Evaluar;
    }
    
    public struct SeleccionAccidentes
    {
        public bool Evaluar;
    }

    public struct SeleccionVolantazos
    {
        public bool Evaluar;
        public float MaxDoblaje;
    }

    public struct SeleccionAceleracion
    {
        public bool Evaluar;
        public float MaxAceleracion;
    }

    public struct SeleccionRpm
    {
        public bool Evaluar;
        public int MaxRpm;
        public int MinRpm;
    }

    public SeleccionTiempo Tiempo;
    public SeleccionCombustible Combustible;
    public SeleccionVelocidad Velocidad;
    public SeleccionSemaforos Semaforos;
    public SeleccionAccidentes Accidentes;
    public SeleccionVolantazos Volantazos;
    public SeleccionAceleracion Aceleracion;
    public SeleccionRpm Rpm;
}