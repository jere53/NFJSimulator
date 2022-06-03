using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecordingManager : MonoBehaviour
{
    
    private Recorder recorder;
    
    private static RecordingManager _instance;
    public static RecordingManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public WeatherController WeatherController;
    public DayNightCicle DayNightCicle;

    #region RecordingHeaderData
    
    private int _captureRate = 24;
    
    [NonSerialized] public List<int> VehicleModelIdList; 
    //cuando se instancian vehiculos agrean su numero de modelo a esta lista para que en la reproduccion los vehiculos
    //tengan los mismos modelos que en la grabacion. Para esto, se requiere conocer el par <IDVehiculo, IDModeloVehiculo>
    //el IDVehiculo es el indice de la lista
        
    [NonSerialized] public List<int> PedestrianModelIdList;
    //idem VehicleModelIdList

    public int streetLightCount;
    
    #endregion
    
    private float _timeBetweenCaptures;
    //segundos hasta la proxima captura

    public delegate void TakeSnapshot(Recorder recorder);

    public event TakeSnapshot OnCaptureSnapshot;

    public string recordingFileName = "";
    public string pathToRecordingsFolder = "";
    
    private float _timeUntilNextSnapshot;

    public bool isRecording;

    private void Awake()
    {
        _instance = this;
        
        recorder = Recorder.instancia;

        PedestrianModelIdList = new List<int>();
        VehicleModelIdList = new List<int>();

        _timeUntilNextSnapshot = 0f;

        if (_captureRate != 0)
        {
            _timeBetweenCaptures = 1f / _captureRate;
        }
        else
        {
            _timeBetweenCaptures = 0; //si no se especifica, capturamos en todos los frames.
        }
    }

    public void StartRecording()
    {
        //settear header todo refactorizar
        recorder.cantidadSemaforos = streetLightCount;
        recorder.modelosPeatones = PedestrianModelIdList;
        recorder.modelosVehiculos = VehicleModelIdList;
        recorder.fps = _captureRate;
        Debug.Log("Recording Started!");
        isRecording = true;
    }


    public void SetCaptureRate(TMP_InputField inputField)
    {
        try
        {
            _captureRate = Math.Abs(int.Parse(inputField.text));
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }

        Debug.Log(_captureRate);
    }
    
    public void StopRecording()
    {
        Debug.Log("Recording Stopped!");
        isRecording = false;
        recorder.escribirHeader();
        recorder.DisposeFS();
    }

    private void Update()
    {
        if(!isRecording) return;
        
        if (_timeUntilNextSnapshot <= 0)
        {
            OnCaptureSnapshot?.Invoke(recorder);
            _timeUntilNextSnapshot = _timeBetweenCaptures;
            recorder.RecordSnapshots();
            recorder.RecordWeatherAndToD();
        }

        _timeUntilNextSnapshot -= Time.deltaTime;
    }
}

public class RecordingHeaderData
{
    public int CaptureRate;
        
    [NonSerialized] public List<int> VehicleModelIdList;

    [NonSerialized] public List<int> PedestrianModelIdList;

    public int streetLightCount;

    public RecordingHeaderData()
    {
        CaptureRate = 0;
        VehicleModelIdList = new List<int>();
        PedestrianModelIdList = new List<int>();
        streetLightCount = 0;
    }
}