using System;
using System.Collections.Generic;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using VehiclePhysics;
using VehiclePhysics.UI;

public class MenuPausa : EscapeDialog
{
    // Enum values for menu
    public enum PauseMenuEnum
    {
        MenuEvaluado,
        MenuEvaluador,
        MenuPausa,
        MenuCondiciones,
        MenuEjercicios,
        MenuPrincipal,
        MenuEvaluacionActual,
        MenuAccidentes,
        MenuGrabacion
    }

    private AudioListener AudioListener; //solamente hay un AudioListener, Unity automaticamente lo asigna

    public static bool estaPausado = false;

    public VPStandardInput controlVehiculo;
    public WeatherController controlClima;
    public VehicleBase VehiculoTrainee;
    
    public GameObject uiMenuEvaluado;
    public GameObject uiMenuEvaluador;
    public GameObject condiciones;
    public GameObject ejercicios;
    public GameObject princial;
    public GameObject evaluacionActual;
    public GameObject accidentes;
    public GameObject grabacion;
    
    // Map for menus
    private Dictionary<PauseMenuEnum, GameObject> _menus = new Dictionary<PauseMenuEnum,GameObject>();
    
    // Save current menu
    private PauseMenuEnum CURRENT_MENU;

    private void Start()
    {
        // Generate a map 
        _menus.Add(PauseMenuEnum.MenuEvaluado, uiMenuEvaluado);
        _menus.Add(PauseMenuEnum.MenuEvaluador, uiMenuEvaluador);
        _menus.Add(PauseMenuEnum.MenuCondiciones, condiciones);
        _menus.Add(PauseMenuEnum.MenuEjercicios, ejercicios);
        _menus.Add(PauseMenuEnum.MenuPrincipal, princial);
        _menus.Add(PauseMenuEnum.MenuEvaluacionActual, evaluacionActual);
        _menus.Add(PauseMenuEnum.MenuAccidentes, accidentes);
        _menus.Add(PauseMenuEnum.MenuGrabacion, grabacion);
        
        DisableAllMenus();
    }
    
    private void DisableAllMenus()
    {
        foreach (var m in _menus)
        {
            m.Value.SetActive(false);
        }
    }

    void Update ()
    {
        if (Input.GetKeyDown(escapeKey))
        {
            if (estaPausado)
                Resumir();
            else
                Pausar();
        }
    }
    
    // private void OnEnable()
    // {
    //     Pausar();
    // }
    //
    // private void OnDisable()
    // {
    //     Resumir();
    // }
    
    private void DesactivarMenuActual()
    {
        _menus[CURRENT_MENU].SetActive(false);
    }
    
    private void ActivarMenuActual()
    {
        Debug.Log(CURRENT_MENU);
        _menus[CURRENT_MENU].SetActive(true);
    }
    
    public void SetCurrentPauseMenu(PauseMenuEnum menu)
    {
        CURRENT_MENU = menu;
        Debug.Log(CURRENT_MENU);
    }
    
    private void Pausar()
    {
        ActivarMenuActual();
        PausarSimulacion();
    }
    
    private void Resumir()
    {
        DesactivarMenuActual();
        ResumirSimulacion();
    }

    public void PausarSimulacion()
    {
        Time.timeScale = 0f;
        AudioListener.pause = true;
        estaPausado = true;
        controlClima.enabled = false;
        controlVehiculo.enabled = false;
    }
    public void ResumirSimulacion()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        estaPausado = false;
        controlClima.enabled = true;
        controlVehiculo.enabled = true;
    }


    public void MenuPrincipalEvaluador()
    {
        princial.SetActive(true);
        condiciones.SetActive(false);
        ejercicios.SetActive(false);
    }
    public void MenuPrincipalGrabador()
    {
        princial.SetActive(true);
        condiciones.SetActive(false);
        ejercicios.SetActive(false);
        evaluacionActual.SetActive(false);
        accidentes.SetActive(false);
        grabacion.SetActive(false);
    }
    public void MenuPrincipalPresentador()
    {
        princial.SetActive(true);
        condiciones.SetActive(false);
        ejercicios.SetActive(false);
        evaluacionActual.SetActive(false);
        accidentes.SetActive(false);
    }
    
    public void Condiciones()
    {
        princial.SetActive(false);
        condiciones.SetActive(true);
    }

    public void Ejercicios()
    {
        princial.SetActive(false);
        ejercicios.SetActive(true);
    }

    public void EvaluacionActual()
    {
        princial.SetActive(false);
        evaluacionActual.SetActive(true);
    }

    public void MenuAccidentes()
    {
        princial.SetActive(false);
        accidentes.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void IrADebriefing()
    {
        SceneManager.LoadScene(1);
    }
    public void CambiarEscena(string nombre)
    {
        SceneManager.LoadScene(nombre);
    }

    public void MenuGrabacion()
    {
        princial.SetActive(false);
        grabacion.SetActive(true);
    }

    public void OnButtonComenzarGrabacionPressed(TMP_InputField inputField)
    {
        if (RecordingManager.Instance.isRecording)
        {
            Debug.LogError("Ya se esta grabando. Si quiere realizar otra grabacion, concluya la actual");
            return;
        }
        
        string initialPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        int captureRate = 24;
        
        try
        {
            captureRate = Math.Abs(int.Parse(inputField.text));
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }

        RecordingManager.Instance.SetCaptureRate(captureRate);
        
        if(FileBrowser.IsOpen) return;
        FileBrowser.SetFilters(true);
        FileBrowser.ShowSaveDialog(ComenzarGrabacion, null, FileBrowser.PickMode.Files,
            false, initialPath);
    }
    
    void ComenzarGrabacion(string[] paths)
    {
        if (!FileBrowser.Success)
        {
            Debug.LogError("Error en el File Browser");
            return;
        }

        string pathToRecordingFile = paths[0];
        string recordingFileName = FileBrowserHelpers.GetFilename(pathToRecordingFile);
        string pathToRecordingFolder = FileBrowserHelpers.GetDirectoryName(pathToRecordingFile) + "\\";

        VehiculoTrainee.gameObject.GetComponent<CapturadorErrores>().enabled = true; //para que comienze a capturar
        //los criterios.
        
        RecordingManager.Instance.StartRecording(pathToRecordingFolder, recordingFileName);
        
    }
    
    public void OnButtonConcluirGrabacionPressed()
    {
        RecordingManager.Instance.StopRecording();
    }
}
