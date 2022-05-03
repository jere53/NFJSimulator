using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICriterio
{
    public void ObtenerDatosEvaluacion(ref DatosEvaluacion datos);

    public void ComenzarEvaluacion();

    public void ConcluirEvaluacion();

    public void Remover();
}

public class DatosEvaluacion
{
    public Tuple<TimeSpan, TimeSpan> DatosCriterioTiempo = null;
    public List<Tuple<float, float, bool>> DatosCriterioVelocidadMaxima = null;
    public List<float> DatosCriterioRespetarSemaforos = null;
    public Tuple<float, float> DatosCriterioNafta = null;
    public Tuple<List<float>, List<Tuple<float, int>>> DatosCriterioEvitarAccidentes = null;

    public void Presentar()
    {
        PresentarTiempo();
        PresentarVelocidadMaxima();
        PresentarRespetarSemaforos();
        PresentarNafta();
        PresentarAccidentes();
    }

    void PresentarTiempo()
    {
        if(DatosCriterioTiempo == null) return;
        
        string resultadoEvaluacion = "";

        TimeSpan tiempoEvaluado = DatosCriterioTiempo.Item1;
        
        TimeSpan tiempoMaximo = DatosCriterioTiempo.Item2;

        if (tiempoEvaluado.Equals(TimeSpan.Zero))
        {
            resultadoEvaluacion =
                "No se definio un tiempo maximo. ";
        }
        else
        {
            resultadoEvaluacion = "El tiempo maximo para completar el ejercicio era: " + tiempoMaximo + ".";
        }

        resultadoEvaluacion += "\n El trainee completo el ejercicio en: " + tiempoEvaluado;
        
        Debug.Log(resultadoEvaluacion);
    }

    void PresentarVelocidadMaxima()
    {
        if(DatosCriterioVelocidadMaxima == null) return;
        
        Debug.Log("Se detectaron los siguientes excesos de velocidad:");
        
        foreach (var infraccion in DatosCriterioVelocidadMaxima)
        {
            if (infraccion.Item3) //si comenzo una infraccion
            {
                Debug.Log("Exceso en el momento " + TimeSpan.FromSeconds(infraccion.Item1) + " velocidad detectada = " + infraccion.Item2);
            }
            else
            { 
                //si concluyo una infraccion
                Debug.Log("Se corrigio un exceso y se volvio a respetar el limite de velocidad en el momento " 
                          + TimeSpan.FromSeconds(infraccion.Item1) + " velocidad detectada = " + infraccion.Item2);
            }
        }
    }

    void PresentarRespetarSemaforos()
    {
        if (DatosCriterioRespetarSemaforos == null) return;

        string resultadoEvaluacion = "El Evaluado paso semaforos en rojo en los siguientes segundos luego de comenzada la evaluacion '\n'";
        foreach (var infraccion in DatosCriterioRespetarSemaforos)
        {
            resultadoEvaluacion += infraccion + '\n';
        }
        Debug.Log(resultadoEvaluacion);
    }

    void PresentarNafta()
    {
        if (DatosCriterioNafta == null) return;
        
        float litrosConsumidos = DatosCriterioNafta.Item1;
        float objetivoLitrosConsumidos = DatosCriterioNafta.Item2;
        
        string resultado = "El objetivo de consumo de combustible eran: " + objetivoLitrosConsumidos + " litros."
                                   + '\n' + "El Evaluado consumio " + litrosConsumidos + " litros";
        
        Debug.Log(resultado);
    }

    void PresentarAccidentes()
    {
        if (DatosCriterioEvitarAccidentes == null) return;
        
        var golpesAPeatones = DatosCriterioEvitarAccidentes.Item1;
        var golpesAVehiculos = DatosCriterioEvitarAccidentes.Item2;
        Debug.Log("Durante la evaluacion, se registraron impactos con peatones en los momentos: \n");
        foreach (var golpe in golpesAPeatones)
        {
            Debug.Log(golpe + "\n");
        }

        Debug.Log("-----------------------");
        Debug.Log("Se registraron golpes contra vehiculos en los momentos: \n");
        foreach (var golpe in golpesAVehiculos)
        {
            Debug.Log(golpe.Item1 + "se golpeo al vehiculo con ID: " + golpe.Item2 + "\n");
        }
    }
}
