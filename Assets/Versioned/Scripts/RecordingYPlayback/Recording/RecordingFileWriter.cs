using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class RecordingFileWriter
{    
    private FileStream _headerFileStream;
    private BinaryWriter _headerWriter;
    
    private FileStream _recordedSnaphotsFileStream;
    private BinaryWriter _snapshotsWriter;
    private FileStream _weatherAndToDFileStream;
    private BinaryWriter _weatherAndToDWriter;

    private FileStream _evalsFileStram;
    private BinaryWriter _evalsWriter;

    private List<DatosEvaluacion> _evals;
    private string _evalsPath;
    
    public RecordingFileWriter(string recordingFolderPath, string recordingName)
    {
        string recordedSnapshotsPath = recordingFolderPath + recordingName + ".nfj";
        if (File.Exists(recordedSnapshotsPath))
            File.Delete(recordedSnapshotsPath);
        _recordedSnaphotsFileStream = File.Create(recordedSnapshotsPath);
        _snapshotsWriter = new BinaryWriter(_recordedSnaphotsFileStream);
        
        string headerPath = recordingFolderPath + recordingName + "Header" + ".nfj";
        if (File.Exists(headerPath))
            File.Delete(headerPath);
        _headerFileStream = File.Create(headerPath);
        _headerWriter = new BinaryWriter(_headerFileStream);
         
        string weatherAndToDPath = recordingFolderPath + recordingName + "WeatherAndToD" + ".nfj";
        if (File.Exists(weatherAndToDPath))
            File.Delete(weatherAndToDPath);
        _weatherAndToDFileStream = File.Create(weatherAndToDPath); 
        _weatherAndToDWriter = new BinaryWriter(_weatherAndToDFileStream);

        _evalsPath = recordingFolderPath + recordingName + "Evals" + ".json";
        _evals = new List<DatosEvaluacion>();
    }

    public void WriteEvalSnapshot(DatosEvaluacion datosEvaluacion)
    {
        _evals.Add(datosEvaluacion);
    }

    private void StoreEvals()
    {
        Debug.Log("storing Evals");
        string output = JsonConvert.SerializeObject(_evals, Formatting.Indented);
        try
        {
            File.WriteAllText(_evalsPath, output);
        }
        catch (Exception exception)
        {
            Debug.LogError(exception);
        }
    }
    
    public void WriteHeader(RecordingHeaderData recordingHeaderData)
    {
        _headerWriter.Write(recordingHeaderData.VehicleModelIdList.Count);
        foreach (int modelo in recordingHeaderData.VehicleModelIdList)
        {
            _headerWriter.Write(modelo);
        }

        _headerWriter.Write(recordingHeaderData.PedestrianModelIdList.Count);
        foreach (int modelo in recordingHeaderData.PedestrianModelIdList)
        {
            _headerWriter.Write(modelo);
        }

        _headerWriter.Write(recordingHeaderData.StreetLightCount);
        _headerWriter.Write(recordingHeaderData.CaptureRate);
        _headerWriter.Write(recordingHeaderData.FrameCount);
        
        StoreEvals();
        DisposeFileHandlers();
    }

    private void DisposeFileHandlers()
    {
        _recordedSnaphotsFileStream.Dispose();
        _weatherAndToDFileStream.Dispose();
        _headerFileStream.Dispose();
    }
    
    public void WriteSnapshot(SnapshotTrainee capturaTrainee, Dictionary<int, SnapshotVehiculo> capturasVehiculos,
        Dictionary<int, SnapshotPeaton> capturasPeatones, Dictionary<int, int> colorSemaforos)
    {
        WriteTransform(capturaTrainee._transform);
        WriteTransform(capturaTrainee.ruedaFL);
        WriteTransform(capturaTrainee.ruedaFR);
        WriteTransform(capturaTrainee.ruedaRL);
        WriteTransform(capturaTrainee.ruedaRR);
        _snapshotsWriter.Write(capturaTrainee.estadoSirena);

        
        _snapshotsWriter.Write(capturasVehiculos.Count);
        foreach(KeyValuePair<int, SnapshotVehiculo> entry in capturasVehiculos)
        {
            _snapshotsWriter.Write(entry.Key);
            WriteTransform(entry.Value._transform);
            WriteTransform(entry.Value.ruedaFL);
            WriteTransform(entry.Value.ruedaFR);
            WriteTransform(entry.Value.ruedaRL);
            WriteTransform(entry.Value.ruedaRR);
        }
        _snapshotsWriter.Write(capturasPeatones.Count);
        foreach(KeyValuePair<int, SnapshotPeaton> entry in capturasPeatones)
        {
            _snapshotsWriter.Write(entry.Key);
            _snapshotsWriter.Write(entry.Value.estaMuerto);
            WriteTransform(entry.Value._transform);
            _snapshotsWriter.Write(entry.Value.velocidad);
        }
        
        foreach(var pair in colorSemaforos.OrderBy(p => p.Key)) {
            _snapshotsWriter.Write(pair.Value);
        }
    }

    private void WriteTransform(Transform transform)
    {
        var position = transform.position;
        _snapshotsWriter.Write(position.x);
        _snapshotsWriter.Write(position.y);
        _snapshotsWriter.Write(position.z);
        var rotation = transform.rotation;
        _snapshotsWriter.Write(rotation.eulerAngles.x);
        _snapshotsWriter.Write(rotation.eulerAngles.y);
        _snapshotsWriter.Write(rotation.eulerAngles.z);
    }

    public void WriteWeatherAndToD(int currentWeather, float currentToD, float currentOrbitSpeed)
    {
        _weatherAndToDWriter.Write(currentWeather);
        _weatherAndToDWriter.Write(currentToD);
        _weatherAndToDWriter.Write(currentOrbitSpeed);
    }
}
