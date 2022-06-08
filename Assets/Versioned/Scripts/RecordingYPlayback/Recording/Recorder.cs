using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Recorder: MonoBehaviour
{
    public Dictionary<int, SnapshotVehiculo> capturasVehiculos;
    public Dictionary<int, SnapshotPeaton> capturasPeatones;
    public SnapshotTrainee capturaTrainee;
    public Dictionary<int, int> colorSemaforos;
    public DatosEvaluacion capturaDatosEvaluacion;

    
    [NonSerialized] public WeatherController instanciaClima; 
    [NonSerialized] public DayNightCicle instanciaCicloDia;

    private RecordingFileWriter _recordingFileWriter;

    public RecordingHeaderData RecordingHeaderData;
    public void Initialize(WeatherController weatherController, DayNightCicle dayNightCicle, 
        string recordingFolderPath, string recordingName)
    {
        instanciaClima = weatherController;
        instanciaCicloDia = dayNightCicle;

        colorSemaforos = new Dictionary<int, int>();
        capturasVehiculos = new Dictionary<int, SnapshotVehiculo>();
        capturasPeatones = new Dictionary<int, SnapshotPeaton>();
        _recordingFileWriter = new RecordingFileWriter(recordingFolderPath, recordingName);
    }
    
    public void StopRecording()
    {
        _recordingFileWriter.WriteHeader(RecordingHeaderData);
        Debug.Log("Se detuvo la grabacion");
    }
    
    public void RecordSnapshots()
    {
        _recordingFileWriter.WriteSnapshot(capturaTrainee, capturasVehiculos, capturasPeatones, colorSemaforos);

        capturasVehiculos.Clear();
        capturasPeatones.Clear();
        colorSemaforos.Clear();
    }

    public void RecordEvals()
    {
        _recordingFileWriter.WriteEvalSnapshot(capturaDatosEvaluacion);
        capturaDatosEvaluacion = null;
    }
    
    public void RecordWeatherAndToD()
    {
        String clima = instanciaClima.currentWeatherPreset;
        int currentWeather = 0;
        switch (clima)
        {
            case "sunny":
                currentWeather = 0;
                break;                
            case "rain":
                currentWeather = 1;
                break;                
            case "cloudy":
                currentWeather = 2;
                break;
        }

        float currentToD = instanciaCicloDia.timeOfDay;
        float currentOrbitSpeed = instanciaCicloDia.orbitSpeed;
        _recordingFileWriter.WriteWeatherAndToD(currentWeather, currentToD, currentOrbitSpeed);
    }
}
