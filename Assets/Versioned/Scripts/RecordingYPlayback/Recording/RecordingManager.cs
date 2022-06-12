using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecordingManager : MonoBehaviour
{
    
    [SerializeField] private Recorder recorder;
    
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

    private float _timeUntilNextSnapshot;

    public bool isRecording;

    private int _frameCount = 0;

    private void Awake()
    {
        _instance = this;

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

    public void StartRecording(string recordingFolderPath, string recordingName)
    {
        recorder.Initialize(WeatherController, DayNightCicle, recordingFolderPath, recordingName);
        Debug.Log("Recording Started!");
        isRecording = true;
    }


    public void SetCaptureRate(int captureRate)
    {
        _captureRate = captureRate;
    }
    
    public void StopRecording()
    {
        isRecording = false;
        RecordingHeaderData recordingHeaderData = new RecordingHeaderData();
        recordingHeaderData.CaptureRate = _captureRate;
        recordingHeaderData.StreetLightCount = streetLightCount;
        recordingHeaderData.PedestrianModelIdList = PedestrianModelIdList;
        recordingHeaderData.VehicleModelIdList = VehicleModelIdList;
        recordingHeaderData.FrameCount = _frameCount;
        recorder.RecordingHeaderData = recordingHeaderData;
        recorder.StopRecording();
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
            recorder.RecordEvals();
            
            _frameCount++;
        }

        _timeUntilNextSnapshot -= Time.deltaTime;
    }
}

public class RecordingHeaderData
{
    public int CaptureRate;

    public List<int> VehicleModelIdList;

    public List<int> PedestrianModelIdList;

    public int StreetLightCount;

    public int FrameCount;

    public RecordingHeaderData()
    {
        CaptureRate = 0;
        VehicleModelIdList = new List<int>();
        PedestrianModelIdList = new List<int>();
        StreetLightCount = 0;
        FrameCount = 0;
    }
}