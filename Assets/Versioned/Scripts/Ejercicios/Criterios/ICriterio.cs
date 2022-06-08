using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
public interface ICriterio
{
    public void EnableRecording(CapturadorErrores capturadorErrores);
    
    public void DisableRecording();

    public void ObtenerDatosEvaluacion(ref DatosEvaluacion datos);

    public void ComenzarEvaluacion();

    public void ConcluirEvaluacion();

    public void Remover();
}

public class DatosCriterioNafta
{
    public float LitrosConsumidos;
    public float ObjetivoListrosConsumidos;
    public List<Tuple<float, float>> ConsumoInstantaneo;

    public List<Tuple<float, float>> ConstumoAccum;
    //tiempo, consumo
    
    public DatosCriterioNafta()
    {
        ConstumoAccum = new List<Tuple<float, float>>();
        ConsumoInstantaneo = new List<Tuple<float, float>>();
        LitrosConsumidos = 0;
        ObjetivoListrosConsumidos = 0;
    }
}

public class DatosCriterioRpm
{
    public List<Tuple<float, int, int, int, bool>> Infracciones;
    //_tiempoActual, rpmActuales, minimoRPM, maximoRPM, corrigioInfraccion
    public List<Tuple<float, float>> RpmEnTiempo;

    public DatosCriterioRpm()
    {
        Infracciones = new List<Tuple<float, int, int, int, bool>>();
        RpmEnTiempo = new List<Tuple<float, float>>();
    }
    
}

public class DatosCriterioVelocidad
{
    public List<Tuple<float, float, bool>> Infracciones;
    //List<_tiempoActual, velocidadActual, corrigioInfraccion>

    public List<Tuple<float, float>> VelocidadEnTiempo;

    public float VelocidadMaxima;

    public DatosCriterioVelocidad()
    {
        Infracciones = new List<Tuple<float, float, bool>>();
        VelocidadEnTiempo = new List<Tuple<float, float>>();
        VelocidadMaxima = 0f;
    }
}

public class DatosCriterioVolantazos
{
    public List<Tuple<float, float, float>> Infracciones;
    //_tiempoActual, doblajeRealizado, doblajeMaximo

    public List<Tuple<float, float>> YawEnTiempo;

    public DatosCriterioVolantazos()
    {
        Infracciones = new List<Tuple<float, float, float>>();
        YawEnTiempo = new List<Tuple<float, float>>();
    }
}

public class DatosCriterioAceleracion
{
    public List<Tuple<float, float, float, float, float>> AceleracionesEnTiempo = null;
    //_timepoActual, Along, Alat, Avert, Amax

    public List<Tuple<float, float, float, float, float>> Infracciones = null;

    public DatosCriterioAceleracion()
    {
        AceleracionesEnTiempo = new List<Tuple<float, float, float, float, float>>();
        Infracciones = new List<Tuple<float, float, float, float, float>>();
    }
}

public class DatosEvaluacion
{
    public Tuple<TimeSpan, TimeSpan> DatosCriterioTiempo = null;
    //TiempoEvaluado, TiempoMaximo

    public List<float> DatosCriterioRespetarSemaforos = null;
    //List<_tiempoActual> (de la infraccion)

    public DatosCriterioNafta DatosCriterioNafta;

    public DatosCriterioRpm DatosCriterioRpm;

    public DatosCriterioVelocidad DatosCriterioVelocidad;

    public DatosCriterioVolantazos DatosCriterioVolantazos;

    public DatosCriterioAceleracion DatosCriterioAceleracion;

    public Tuple<List<float>, List<Tuple<float, int>>> DatosCriterioEvitarAccidentes = null;
    //List<_tiempoActual> (peatones), List<_tiempoActual, InstanceID> (vehiculos)

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
        if(DatosCriterioTiempo.Item1 == TimeSpan.Zero && DatosCriterioTiempo.Item2 == TimeSpan.Zero) return;
        
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
        if(DatosCriterioVelocidad == null) return;
        if(DatosCriterioVelocidad.Infracciones.Count == 0) return;
        
        Debug.Log("Se detectaron los siguientes excesos de velocidad:");
        
        foreach (var infraccion in DatosCriterioVelocidad.Infracciones)
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
        if (DatosCriterioRespetarSemaforos.Count == 0) return;

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
        if (DatosCriterioNafta.LitrosConsumidos <= DatosCriterioNafta.ObjetivoListrosConsumidos) return;
        
        float litrosConsumidos = DatosCriterioNafta.LitrosConsumidos;
        float objetivoLitrosConsumidos = DatosCriterioNafta.ObjetivoListrosConsumidos;
        
        string resultado = "El objetivo de consumo de combustible eran: " + objetivoLitrosConsumidos + " litros."
                                   + '\n' + "El Evaluado consumio " + litrosConsumidos + " litros";
        
        Debug.Log(resultado);
    }

    void PresentarAccidentes()
    {
        if (DatosCriterioEvitarAccidentes == null) return;
        if (DatosCriterioEvitarAccidentes.Item1.Count == 0 && DatosCriterioEvitarAccidentes.Item2.Count == 0) return;

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
        if(DatosCriterioRpm == null) return;
        if (DatosCriterioRpm.Infracciones.Count == 0) return;
        
        Debug.Log("Se detectaron las siguientes faltas en RPM:");
        
        foreach (var infraccion in DatosCriterioRpm.Infracciones)
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
        if (DatosCriterioAceleracion.Infracciones.Count == 0) return;

        Debug.Log("Se detectaron los siguientes errores respecto a la aceleracion: ");

        foreach (var infraccion in DatosCriterioAceleracion.Infracciones)
        {
            Debug.Log("La aceleracion maxima, en Gs, era: " + infraccion.Item5 + "\n");
            Debug.Log("Las aceleraciones registradas al momento " + infraccion.Item1 + 
                      "fueron: \n Longitudinal: " + infraccion.Item2 + "\n Lateral: " + infraccion.Item3 + 
                      "\n Vertical: " + infraccion.Item4);
        }
    }

    void PresentarVolantazo()
    {
        if (DatosCriterioVolantazos == null) return;
        if (DatosCriterioVolantazos.Infracciones.Count == 0) return;
        
        Debug.Log("Volantazos: ");
        foreach (var infraccion in DatosCriterioVolantazos.Infracciones)
        {
            Debug.Log("En el momento " + infraccion.Item1 + " se dio un volantazo de " + 
                      infraccion.Item2 + " radianes/seg." +
                      "El maximo volantazo seguro en esa cantidad de segundos era " + infraccion.Item3 + "rads/s \n");
        }
    }

    public void GuardarEnDisco(string path)
    {
        string output = JsonConvert.SerializeObject(this);
        try
        {
            File.WriteAllText(path, output);
        }
        catch (Exception exception)
        {
            Debug.LogError(exception);
        }
    }

    public static DatosEvaluacion CargarDeDisco(string path)
    {
        DatosEvaluacion res = new DatosEvaluacion();
        try
        {
            var inputString = File.ReadAllText(path);
            res = JsonConvert.DeserializeObject<DatosEvaluacion>(inputString);
        }
        catch (Exception exception)
        {
            Debug.LogWarning("Se produjo un error al intentar cargar el archivo");
            Debug.LogWarning(exception);
        }
        return res;
    }
}
