using System;
using System.IO;
using SimpleFileBrowser;
using UnityEngine;

public class MenuPlayback : MonoBehaviour
{
    public ControladorGrabacion ControladorGrabacion;
    
    public void OnButtonPlayPressed()
    {

        string initialPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        if(FileBrowser.IsOpen) return;
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Grabaciones", ".nfj"));
        FileBrowser.SetDefaultFilter(".nfj");
        FileBrowser.ShowLoadDialog(ComenzarPlayback, null, FileBrowser.PickMode.Files,
            false, initialPath);
    }

    private void ComenzarPlayback(string[] paths)
    {
        string path = paths[0];

        string pathToRecordingFolder = FileBrowserHelpers.GetDirectoryName(path) + "\\";
        string recordingName = FileBrowserHelpers.GetFilename(path);
        
        string pathToRecording = pathToRecordingFolder + recordingName;
        string pathToHeader = pathToRecordingFolder + Path.GetFileNameWithoutExtension(recordingName) + "Header.nfj";
        string pathToWeatherAndToDRecording = pathToRecordingFolder + Path.GetFileNameWithoutExtension(recordingName) + "WeatherAndToD.nfj";
        string pathToEvalRecording = pathToRecordingFolder + Path.GetFileNameWithoutExtension(recordingName) +
                              "Evals.json";

        ControladorGrabacion.pathClima = pathToWeatherAndToDRecording;
        ControladorGrabacion.pathGrabacion = pathToRecording;
        ControladorGrabacion.pathHeader = pathToHeader;
        ControladorGrabacion.pathToEvals = pathToEvalRecording;
        
        ControladorGrabacion.ComenzarPlayback(1, false);
    }
}
