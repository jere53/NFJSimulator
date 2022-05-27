using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using TMPro;

public class DebrieferPresenter : MonoBehaviour
{
    private string _pathToMyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    public TMP_InputField pathArchivoEvaluacion;

    public DebrieferModel debrieferModel;
    
    public void OnButtonSeleccionarEvaluacionPressed()
    {
        if(FileBrowser.IsOpen) return;
        FileBrowser.SetFilters(true, new FileBrowser.Filter("JSONs", ".json"));
        FileBrowser.SetDefaultFilter(".json");
        FileBrowser.ShowLoadDialog(OnFileBrowserSuccess, null, FileBrowser.PickMode.Files,
            false, _pathToMyDocuments, null, "Load", "Select");
    }

    void OnFileBrowserSuccess(string[] paths)
    {
        pathArchivoEvaluacion.text = paths[0];
    }

    public void OnButtonDebriefPressed()
    {
        debrieferModel.CargarDatosEvaluacion(pathArchivoEvaluacion.text);
    }

    public void OnGraficoAceleracion()
    {
        
    }
}
