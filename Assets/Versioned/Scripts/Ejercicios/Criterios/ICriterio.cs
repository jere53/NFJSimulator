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
    //TiempoEvaluado, TiempoMaximo
    
    public List<Tuple<float, float, bool>> DatosCriterioVelocidadMaxima = null;
    //List<_tiempoActual, velocidadActual, corrigioInfraccion>
    
    public List<float> DatosCriterioRespetarSemaforos = null;
    //List<_tiempoActual> (de la infraccion)
    
    
    public Tuple<float, float> DatosCriterioNafta = null;
    //_litrosConsumidos, _objetivoLitrosConsumidos 
    
    public Tuple<List<float>, List<Tuple<float, int>>> DatosCriterioEvitarAccidentes = null;
    //List<_tiempoActual> (peatones), List<_tiempoActual, InstanceID> (vehiculos)
    
    public List<Tuple<float, int, int, int, bool>> DatosCriterioRPM = null;
    //_tiempoActual, rpmActuales, minimoRPM, maximoRPM, corrigioInfraccion

    public List<Tuple<float, float, float, float, float>> DatosCriterioAceleracion = null;
    //_timepoActual, Along, Alat, Avert, Amax
    
    public List<Tuple<float, float, float>> DatosCriterioVolantazo = null;
    //_tiempoActual, doblajeRealizado, doblajeMaximo, granularidad
    
    public void Presentar()
    {
        PresentarTiempo();
        PresentarVelocidadMaxima();
        PresentarRespetarSemaforos();
        PresentarNafta();
        PresentarAccidentes();
        PresentarRPM();
        PresentarAceleracion();
        PresentarVolantazo();
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

    void PresentarRPM()
    {
        if(DatosCriterioRPM == null) return;
        
        Debug.Log("Se detectaron las siguientes faltas en RPM:");
        
        foreach (var infraccion in DatosCriterioRPM)
        {
            if (infraccion.Item5) //si comenzo una infraccion
            {
                Debug.Log("Falta en el momento: " + TimeSpan.FromSeconds(infraccion.Item1) + " RPMs detectadas = " 
                          + infraccion.Item2 + "\n Rango aceptado: " + infraccion.Item3 + "; " + infraccion.Item4);
            }
            else
            { 
                //si concluyo una infraccion
                Debug.Log("Se corrigio una falta y se volvio a respetar el limite de RPM en el momento: " 
                          + TimeSpan.FromSeconds(infraccion.Item1) + " RPMs detectadas = " + infraccion.Item2
                          + "\n Rango aceptado: " + infraccion.Item3 + "; " + infraccion.Item4);
            }
        }
    }

    void PresentarAceleracion()
    {
        if (DatosCriterioAceleracion == null) return;

        Debug.Log("Se detectaron los siguientes errores respecto a la aceleracion: ");

        foreach (var infraccion in DatosCriterioAceleracion)
        {
            Debug.Log("La aceleracion maxima, en Gs, era: " + infraccion.Item5 + "\n");
            Debug.Log("Las aceleraciones registradas al momento " + infraccion.Item1 + 
                      "fueron: \n Longitudinal: " + infraccion.Item2 + "\n Lateral: " + infraccion.Item3 + 
                      "\n Vertical: " + infraccion.Item4);
        }
    }

    void PresentarVolantazo()
    {
        Debug.Log("Volantazos: ");
        foreach (var infraccion in DatosCriterioVolantazo)
        {
            Debug.Log("En el momento " + infraccion.Item1 + " se dio un volantazo de " + 
                      infraccion.Item2 + " radianes/seg." +
                      "El maximo volantazo seguro en esa cantidad de segundos era " + infraccion.Item3 + "rads/s \n");
        }
    }
}
