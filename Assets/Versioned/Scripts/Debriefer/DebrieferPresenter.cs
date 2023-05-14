using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SimpleFileBrowser;
using TMPro;
using UnityEngine.SceneManagement;

public class DebrieferPresenter : MonoBehaviour
{
    private enum OpcionAceleracion
    {
        Longitudianal,
        Lateral,
        Vertical
    }

    private string _pathToMyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    public TMP_InputField pathArchivoEvaluacion;

    public DebrieferModel debrieferModel;

    //cosas del grafico
    private Window_Graph _windowGraph;
    public Sprite lineGraphDotSprite;

    public void OnButtonSeleccionarEvaluacionPressed()
    {
        if(FileBrowser.IsOpen) return;
        FileBrowser.SetFilters(true, new FileBrowser.Filter("JSONs", ".json"));
        FileBrowser.SetDefaultFilter(".json");
        FileBrowser.ShowLoadDialog(OnFileBrowserSuccess, null, FileBrowser.PickMode.Files,
            false, _pathToMyDocuments);
    }

    void OnFileBrowserSuccess(string[] paths)
    {
        pathArchivoEvaluacion.text = paths[0];
    }
    public void CambiarEscena(string nombre)
    {
        SceneManager.LoadScene(nombre);
    }
    public void OnButtonDebriefPressed()
    {
        debrieferModel.CargarDatosEvaluacion(pathArchivoEvaluacion.text);
    }

    #region MostrarAceleracion
    public void OnMostrarAceleracionLongitudinal(Window_Graph windowGraph)
    {
        _windowGraph = windowGraph;
        MostrarGraficoAceleracion(OpcionAceleracion.Longitudianal);
    }

    public void OnMostrarAceleracionLateral(Window_Graph windowGraph)
    {
        _windowGraph = windowGraph;
        MostrarGraficoAceleracion(OpcionAceleracion.Lateral);
    }

    public void OnMostrarAceleracionVertical(Window_Graph windowGraph)
    {
        _windowGraph = windowGraph;
        MostrarGraficoAceleracion(OpcionAceleracion.Vertical);
    }
    
    public void OnMostrarInfraccionesAceleracion(Window_Graph windowGraph)
    {
        _windowGraph = windowGraph;
        MostrarInfraccionesAceleracion();
    }
    private void MostrarInfraccionesAceleracion()
    {
        var datosAceleracion = debrieferModel.GetDatosAceleracion();
        if (datosAceleracion == null)
        {
            Debug.Log("No hay datos de aceleracion que mostrar");
            return;
        }
        
        List<float> xAxisLabelList = new List<float>();
        List<float> valueList = new List<float>();
        
        int cantidadInfracciones = 0;
        foreach (var dato in datosAceleracion.Infracciones)
        {
            xAxisLabelList.Add(dato.Item1);
            cantidadInfracciones++;
            valueList.Add(cantidadInfracciones);
        }

        Debug.LogWarning(cantidadInfracciones);
        
        Window_Graph.LineGraphVisual lineGraphVisual = new Window_Graph.LineGraphVisual(
            _windowGraph,
            _windowGraph.GetGraphContainer(), 
            lineGraphDotSprite, 
            Color.green, 
            new Color(1,1,1,.5f)
            );
        
        _windowGraph.ShowGraph(
            valueList, 
            xAxisLabelList,
            lineGraphVisual, 
            -1,
            f =>
            {
                var s = (f).ToString("n2");
                return s;
            },
            f =>
            {
                var s = f.ToString("n2");
                return s;
            }
            );
    }
    
    private void MostrarGraficoAceleracion(OpcionAceleracion opcionAceleracion)
    {
        var datosAceleracion = debrieferModel.GetDatosAceleracion();
        if (datosAceleracion == null)
        {
            Debug.Log("No hay datos de aceleracion que mostrar");
            return;
        }
        //tiempo, Along, Alat, Avert, Amax
        List<float> xAxisLabelList = new List<float>();
        foreach (var dato in datosAceleracion.AceleracionesEnTiempo)
        {
            xAxisLabelList.Add((dato.Item1));
        }

        List<float> valueList = new List<float>();
        foreach (var dato in datosAceleracion.AceleracionesEnTiempo)
        {
            switch (opcionAceleracion)
            {
                case OpcionAceleracion.Longitudianal:
                    valueList.Add(dato.Item2);
                    continue;
                case OpcionAceleracion.Lateral:
                    valueList.Add(dato.Item3);
                    continue;
                case OpcionAceleracion.Vertical:
                    valueList.Add(dato.Item4);
                    continue;
            }
        }
        
        Window_Graph.LineGraphVisual lineGraphVisual = new Window_Graph.LineGraphVisual(
            _windowGraph,
            _windowGraph.GetGraphContainer(), 
            lineGraphDotSprite, 
            Color.green, 
            new Color(1,1,1,.5f)
        );
        
        _windowGraph.ShowGraph(
            valueList, 
            xAxisLabelList,
            lineGraphVisual, 
            -1,
            f =>
            {
                var s = (f).ToString("n2");
                return s;
            },
            f =>
            {
                var s = f.ToString("n2");
                return s;
            }
        );
    }
    
    public void OnMostrarTextoAceleracion(TextMeshProUGUI textMeshPro)
    {
        var datosAceleracion = debrieferModel.GetDatosAceleracion();
        
        if (datosAceleracion == null)
        {
            Debug.Log("No hay datos de aceleracion que mostrar");
            return;
        }
        
        int cantInfracciones = datosAceleracion.Infracciones.Count;

        textMeshPro.text = ("Hubo " + cantInfracciones + " excesos en la aceleracion.\n");
    }


    #endregion

    #region MostrarAccidentes

    private void MostrarGraficoAccidentes()
    {
        var datosAccidentes = debrieferModel.GetDatosAccidentes();
        if (datosAccidentes == null)
        {
            Debug.Log("No hay datos de accidentes que mostrar");
            return;
        }

        List<float> momentosAccidentes = new List<float>();
        foreach (var momentoPeatonGolpeado in datosAccidentes.Item1)
        {
            momentosAccidentes.Add(momentoPeatonGolpeado);
            
        }

        foreach (var momentoVehiculoGolpeado in datosAccidentes.Item2)
        {
            momentosAccidentes.Add(momentoVehiculoGolpeado.Item1);
        }

        momentosAccidentes.Sort();
        
        List<float> valueList = new List<float>();
        int cantidadAccidentes = 0;
        foreach (var momento in momentosAccidentes)
        {
            cantidadAccidentes++;
            valueList.Add(cantidadAccidentes);
        }

        List<float> xAxisLabelList = momentosAccidentes;
        
        Window_Graph.LineGraphVisual lineGraphVisual = new Window_Graph.LineGraphVisual(
            _windowGraph,
            _windowGraph.GetGraphContainer(), 
            lineGraphDotSprite, 
            Color.green, 
            new Color(1,1,1,.5f)
        );
        
        _windowGraph.ShowGraph(
            valueList, 
            xAxisLabelList,
            lineGraphVisual, 
            -1,
            f =>
            {
                var s = (f).ToString("n2");
                return s;
            },
            f =>
            {
                var s = f.ToString("n2");
                return s;
            }
        );
    }

    public void OnMostrarTextoAccidentes(TextMeshProUGUI textMeshPro)
    {
        var datosAccidentes = debrieferModel.GetDatosAccidentes();
        
        if (datosAccidentes == null)
        {
            Debug.Log("No hay datos de accidentes que mostrar");
            return;
        }
        
        int cantidadAccidentes = datosAccidentes.Item1.Count + datosAccidentes.Item2.Count;
        
        List<int> idVehiculos = new List<int>();

        foreach (var colision in datosAccidentes.Item2)
        {
            idVehiculos.Add(colision.Item2);
        }
        
        textMeshPro.text = ("Hubo " + cantidadAccidentes + " accidentes.\n"
            + "Se detectaron " + datosAccidentes.Item1.Count + " colisiones contra peatones.\n"
            + "Se detectaron " + datosAccidentes.Item2.Count + " colisiones contra vehiculos.\n"
            + "Se colisiono contra " + idVehiculos.Distinct().Count() + " vehiculos diferentes.");
    }
    public void OnMostarGraficoAccidentes(Window_Graph windowGraph)
    {
        _windowGraph = windowGraph;
        MostrarGraficoAccidentes();
    }


    #endregion

    #region MostrarCombustible

    public void OnMostrarConsumoInstantaneo(Window_Graph windowGraph)
    {
        _windowGraph = windowGraph;
        MostrarConsumoInstantaneo();
    }
    public void MostrarConsumoInstantaneo()
    {
        var datosConsumo = debrieferModel.GetDatosConsumo();
        if (datosConsumo == null)
        {
            Debug.Log("No hay datos de consumo que mostrar");
            return;
        }
        //tiempo, consumo
        
        List<float> xAxisLabelList = new List<float>();
        List<float> valueList = new List<float>();

        foreach (var dato in datosConsumo.ConsumoInstantaneo)
        {
            xAxisLabelList.Add((dato.Item1));
            valueList.Add(dato.Item2);
        }

        
        Window_Graph.LineGraphVisual lineGraphVisual = new Window_Graph.LineGraphVisual(
            _windowGraph,
            _windowGraph.GetGraphContainer(), 
            lineGraphDotSprite, 
            Color.green, 
            new Color(1,1,1,.5f)
        );
        
        _windowGraph.ShowGraph(
            valueList, 
            xAxisLabelList,
            lineGraphVisual, 
            -1,
            f =>
            {
                var s = (f).ToString("n2");
                return s;
            },
            f =>
            {
                var s = f.ToString("n2");
                return s;
            }
        );
    }

    public void OnMostrarConsumoAccum(Window_Graph windowGraph)
    {
        _windowGraph = windowGraph;
        MostrarConsumoAccum();
    }
    public void MostrarConsumoAccum()
    {
        var datosConsumo = debrieferModel.GetDatosConsumo();
        if (datosConsumo == null)
        {
            Debug.Log("No hay datos de consumo que mostrar");
            return;
        }
        //tiempo, consumo
        
        List<float> xAxisLabelList = new List<float>();
        List<float> valueList = new List<float>();

        foreach (var dato in datosConsumo.ConstumoAccum)
        {
            xAxisLabelList.Add((dato.Item1));
            valueList.Add(dato.Item2);
        }

        
        Window_Graph.LineGraphVisual lineGraphVisual = new Window_Graph.LineGraphVisual(
            _windowGraph,
            _windowGraph.GetGraphContainer(), 
            lineGraphDotSprite, 
            Color.green, 
            new Color(1,1,1,.5f)
        );
        
        _windowGraph.ShowGraph(
            valueList, 
            xAxisLabelList,
            lineGraphVisual, 
            -1,
            f =>
            {
                var s = (f).ToString("n2");
                return s;
            },
            f =>
            {
                var s = f.ToString("n2");
                return s;
            }
        );
    } 
    
    public void OnMostrarTextoConsumo(TextMeshProUGUI textMeshPro)
    {
        var datosConsumo = debrieferModel.GetDatosConsumo();
        
        if (datosConsumo == null)
        {
            Debug.Log("No hay datos de consumo que mostrar");
            return;
        }

        float consumoPromedio = 0;
        foreach (var c in datosConsumo.ConsumoInstantaneo)
        {
            Debug.Log(c.Item2);
            consumoPromedio += c.Item2;
        }

        consumoPromedio /= datosConsumo.ConsumoInstantaneo.Count;

        textMeshPro.text = ("Se consumieron " + datosConsumo.LitrosConsumidos + " litros de nafta en total.\n"
                            + "El consumo instantaneo promedio fue " + consumoPromedio + " litros de nafta.\n"
                            + "El objetivo de consumo de nafta eran " + datosConsumo.ObjetivoListrosConsumidos +
                            " litros.\n");
    }
    
    #endregion

    #region MostrarSemaforos

    public void OnMostrarInfraccionesSemaforos(Window_Graph windowGraph)
    {
        _windowGraph = windowGraph;
        MostrarInfraccionesSemaforos();
    }
    public void MostrarInfraccionesSemaforos()
    {
        var datosSemaforo = debrieferModel.GetDatosSemaforo();
        if (datosSemaforo == null)
        {
            Debug.Log("No hay datos de semaforo que mostrar");
            return;
        }
        //tiempo, consumo
        
        List<float> xAxisLabelList = new List<float>();
        List<float> valueList = new List<float>();

        int cantidadInfracciones = 0;
        foreach (var dato in datosSemaforo)
        {
            cantidadInfracciones++;
            xAxisLabelList.Add(dato);
            valueList.Add(cantidadInfracciones);
        }

        
        Window_Graph.LineGraphVisual lineGraphVisual = new Window_Graph.LineGraphVisual(
            _windowGraph,
            _windowGraph.GetGraphContainer(), 
            lineGraphDotSprite, 
            Color.green, 
            new Color(1,1,1,.5f)
        );
        
        _windowGraph.ShowGraph(
            valueList, 
            xAxisLabelList,
            lineGraphVisual, 
            -1,
            f =>
            {
                var s = (f).ToString("n2");
                return s;
            },
            f =>
            {
                var s = f.ToString("n2");
                return s;
            }
        );
    } 

    public void OnMostrarTextoSemaforos(TextMeshProUGUI textMeshPro)
    {
        var datosSemaforo = debrieferModel.GetDatosSemaforo();
        
        if (datosSemaforo == null)
        {
            Debug.Log("No hay datos de semaforo que mostrar");
            return;
        }

        textMeshPro.text = ("Hubo " + datosSemaforo.Count + " semaforos por los que se paso en rojo.");
    }

    #endregion

    #region MostrarRpm

    public void OnMostrarRpmEnTiempo(Window_Graph windowGraph)
    {
        _windowGraph = windowGraph;
        MostrarRpmEnTiempo();
    }
    public void MostrarRpmEnTiempo()
    {
        var datosRpm = debrieferModel.GetDatosRpm();
        if (datosRpm == null)
        {
            Debug.Log("No hay datos de RPM que mostrar");
            return;
        }
        
        List<float> xAxisLabelList = new List<float>();
        List<float> valueList = new List<float>();

        foreach (var dato in datosRpm.RpmEnTiempo)
        {
            xAxisLabelList.Add(dato.Item1);
            valueList.Add(dato.Item2);
        }

        
        Window_Graph.LineGraphVisual lineGraphVisual = new Window_Graph.LineGraphVisual(
            _windowGraph,
            _windowGraph.GetGraphContainer(), 
            lineGraphDotSprite, 
            Color.green, 
            new Color(1,1,1,.5f)
        );
        
        _windowGraph.ShowGraph(
            valueList, 
            xAxisLabelList,
            lineGraphVisual, 
            -1,
            f =>
            {
                var s = (f).ToString("n2");
                return s;
            },
            f =>
            {
                var s = f.ToString("n2");
                return s;
            }
        );
    }

    public void OnMostrarInfraccionesRpm(Window_Graph windowGraph)
    {
        _windowGraph = windowGraph;
        MostrarInfraccionesRpm();
    }
    public void MostrarInfraccionesRpm()
    {
        var datosRpm = debrieferModel.GetDatosRpm();
        if (datosRpm == null)
        {
            Debug.Log("No hay datos de RPM que mostrar");
            return;
        }
        
        List<float> xAxisLabelList = new List<float>();
        List<float> valueList = new List<float>();

        int cantidadDeInfracciones = 0;
        foreach (var dato in datosRpm.Infracciones)
        {
            if(!dato.Item5) continue;
            //si se estaba corrigiendo una infraccion, no se grafica.
            cantidadDeInfracciones++;
            xAxisLabelList.Add(dato.Item1);
            valueList.Add(cantidadDeInfracciones);
        }

        
        Window_Graph.LineGraphVisual lineGraphVisual = new Window_Graph.LineGraphVisual(
            _windowGraph,
            _windowGraph.GetGraphContainer(), 
            lineGraphDotSprite, 
            Color.green, 
            new Color(1,1,1,.5f)
        );
        
        _windowGraph.ShowGraph(
            valueList, 
            xAxisLabelList,
            lineGraphVisual, 
            -1,
            f =>
            {
                var s = (f).ToString("n2");
                return s;
            },
            f =>
            {
                var s = f.ToString("n2");
                return s;
            }
        );
    }

    public void OnMostrarTextoRpm(TextMeshProUGUI textMeshPro)
    {
        var datosRpm = debrieferModel.GetDatosRpm();
        if (datosRpm == null)
        {
            Debug.Log("No hay datos de RPM que mostrar");
            return;
        }

        int cantInfracciones = 0;
        foreach (var infraccion in datosRpm.Infracciones)
        {
            if (infraccion.Item5) cantInfracciones++;
            //solo sumamos cuando estaba marcando el comienzo de una infraccion.
        }


        textMeshPro.text = ("Hubo " + cantInfracciones + " excesos/faltas en las RPM");
    }


    #endregion

    #region MostrarTiempo

    public void OnMostrarTextoTiempo(TextMeshProUGUI textMeshPro)
    {
        var datosTiempo = debrieferModel.GetDatosTiempo();
        if (datosTiempo == null)
        {
            Debug.Log("No hay datos de tiempo que mostrar");
            return;
        }


        textMeshPro.text = ("El trainee completo la evaluacion en " + datosTiempo.Item1 + "\n"+
                            "El tiempo maximo permitido era: " + datosTiempo.Item2);
    }

    #endregion

    #region MostrarVelocidad

    public void OnMostrarInfraccionesVelocidad(Window_Graph windowGraph)
    {
        _windowGraph = windowGraph;
        MostrarInfraccionesVelocidad();
    }
    public void MostrarInfraccionesVelocidad()
    {
        var datosVelocidad = debrieferModel.GetDatosVelocidad();
        if (datosVelocidad == null)
        {
            Debug.Log("No hay datos de velocidad que mostrar");
            return;
        }
        //tiempo, consumo
        
        List<float> xAxisLabelList = new List<float>();
        List<float> valueList = new List<float>();

        int cantidadInfracciones = 0;
        foreach (var dato in datosVelocidad.Infracciones)
        {
            cantidadInfracciones++;
            xAxisLabelList.Add(dato.Item1);
            valueList.Add(cantidadInfracciones);
        }

        
        Window_Graph.LineGraphVisual lineGraphVisual = new Window_Graph.LineGraphVisual(
            _windowGraph,
            _windowGraph.GetGraphContainer(), 
            lineGraphDotSprite, 
            Color.green, 
            new Color(1,1,1,.5f)
        );
        
        _windowGraph.ShowGraph(
            valueList, 
            xAxisLabelList,
            lineGraphVisual, 
            -1,
            f =>
            {
                var s = (f).ToString("n2");
                return s;
            },
            f =>
            {
                var s = f.ToString("n2");
                return s;
            }
        );
    }

    public void OnMostrarVelocidadEnTiempo(Window_Graph windowGraph)
    {
        _windowGraph = windowGraph;
        MostrarVelocidadEnTiempo();
    }
    public void MostrarVelocidadEnTiempo()
    {
        var datosVelocidad = debrieferModel.GetDatosVelocidad();
        if (datosVelocidad == null)
        {
            Debug.Log("No hay datos de velocidad que mostrar");
            return;
        }
        
        List<float> xAxisLabelList = new List<float>();
        List<float> valueList = new List<float>();

        foreach (var dato in datosVelocidad.VelocidadEnTiempo)
        {
            xAxisLabelList.Add(dato.Item1);
            valueList.Add(dato.Item2);
        }

        
        Window_Graph.LineGraphVisual lineGraphVisual = new Window_Graph.LineGraphVisual(
            _windowGraph,
            _windowGraph.GetGraphContainer(), 
            lineGraphDotSprite, 
            Color.green, 
            new Color(1,1,1,.5f)
        );
        
        _windowGraph.ShowGraph(
            valueList, 
            xAxisLabelList,
            lineGraphVisual, 
            -1,
            f =>
            {
                var s = (f).ToString("n2");
                return s;
            },
            f =>
            {
                var s = f.ToString("n2");
                return s;
            }
        );
    } 

    public void OnMostrarTextoVelocidad(TextMeshProUGUI textMeshPro)
    {
        var datosVelocidad = debrieferModel.GetDatosVelocidad();
        
        if (datosVelocidad == null)
        {
            Debug.Log("No hay datos de velocidad que mostrar");
            return;
        }

        string text = "La velocidad maxima permitida era de " + datosVelocidad.VelocidadMaxima + " KM/h";
        foreach (var infraccion in datosVelocidad.Infracciones)
        {
            if (!infraccion.Item3)
            {
                text += "Se excedio la velocidad maxima permitida (el trainee iba a " + infraccion.Item2 +  " KM/h) a los " +
                        infraccion.Item1 + " segundos de comenzada la evaluacion\n";
            }
            else
            {
                text += "Se corrigio un exceso en la velocidad maxima permitida (el trainee iba a " + infraccion.Item2 + " KM/h) a los " +
                        infraccion.Item1 + " segundos de comenzada la evaluacion\n";
            }
        }

        textMeshPro.text = text;

    }

    #endregion

    #region MostrarVolantazosEnTiempo
    
    public void OnMostrarInfraccionesVolantazos(Window_Graph windowGraph)
    {
        _windowGraph = windowGraph;
        MostrarInfraccionesVolantazos();
    }
    
    public void MostrarInfraccionesVolantazos()
    {
        var datosVolantazos = debrieferModel.GetDatosVolantazos();
        if (datosVolantazos == null)
        {
            Debug.Log("No hay datos de volantazos que mostrar");
            return;
        }

        List<float> xAxisLabelList = new List<float>();
        List<float> valueList = new List<float>();

        int cantidadInfracciones = 0;
        foreach (var dato in datosVolantazos.Infracciones)
        {
            cantidadInfracciones++;
            xAxisLabelList.Add(dato.Item1);
            valueList.Add(cantidadInfracciones);
        }

        
        Window_Graph.LineGraphVisual lineGraphVisual = new Window_Graph.LineGraphVisual(
            _windowGraph,
            _windowGraph.GetGraphContainer(), 
            lineGraphDotSprite, 
            Color.green, 
            new Color(1,1,1,.5f)
        );
        
        _windowGraph.ShowGraph(
            valueList, 
            xAxisLabelList,
            lineGraphVisual, 
            -1,
            f =>
            {
                var s = (f).ToString("n2");
                return s;
            },
            f =>
            {
                var s = f.ToString("n2");
                return s;
            }
        );
    }


    public void OnMostrarVolantazosEnTiempo(Window_Graph windowGraph)
    {
        _windowGraph = windowGraph;
        MostrarVolantazosEnTiempo();
    }
    public void MostrarVolantazosEnTiempo()
    {
        var datosVolantazos = debrieferModel.GetDatosVolantazos();
        if (datosVolantazos == null)
        {
            Debug.Log("No hay datos de volantazos que mostrar");
            return;
        }

        List<float> xAxisLabelList = new List<float>();
        List<float> valueList = new List<float>();

        foreach (var dato in datosVolantazos.YawEnTiempo)
        {
            xAxisLabelList.Add(dato.Item1);
            valueList.Add(dato.Item2);
        }

        
        Window_Graph.LineGraphVisual lineGraphVisual = new Window_Graph.LineGraphVisual(
            _windowGraph,
            _windowGraph.GetGraphContainer(), 
            lineGraphDotSprite, 
            Color.green, 
            new Color(1,1,1,.5f)
        );
        
        _windowGraph.ShowGraph(
            valueList, 
            xAxisLabelList,
            lineGraphVisual, 
            -1,
            f =>
            {
                var s = (f).ToString("n2");
                return s;
            },
            f =>
            {
                var s = f.ToString("n2");
                return s;
            }
        );
    } 
    public void OnMostrarTextoVolantazos(TextMeshProUGUI textMeshPro)
    {
        var datosVolantazos = debrieferModel.GetDatosVolantazos();
        
        if (datosVolantazos == null)
        {
            Debug.Log("No hay datos de volantazos que mostrar");
            return;
        }

        string text = "Se detectaron las siguientes infracciones relacionadas a dar volantazos: \n ";

        foreach (var infraccion in datosVolantazos.Infracciones)
        {
            text += "A los " + infraccion.Item1 + " segundos de comenzada la evaluacion, se detecto una " +
                    "velocidad de giro de " + infraccion.Item2 + "rad/s; el maximo giro seguro es de " +
                    infraccion.Item3 + "rad/s\n";
        }

        textMeshPro.text = text;
    }

    #endregion
}

