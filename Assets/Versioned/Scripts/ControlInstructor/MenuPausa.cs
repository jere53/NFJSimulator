using UnityEngine;
using UnityEngine.SceneManagement;
using VehiclePhysics;
using VehiclePhysics.UI;

public class MenuPausa : EscapeDialog
{

    private AudioListener AudioListener; //solamente hay un AudioListener, Unity automaticamente lo asigna

    public static bool estaPausado = false;

    public VPStandardInput controlVehiculo;
    public WeatherController controlClima;
    public CapturadorTrainee controlGrabacion;
    
    
    public GameObject uiMenuPausa;
    public GameObject condiciones;
    public GameObject ejercicios;
    public GameObject princial;
    public GameObject evaluacionActual;
    public GameObject accidentes;

    void Update ()
    {
        if (Input.GetKeyDown(escapeKey))
            this.gameObject.SetActive(false);
    }
    
    private void OnEnable()
    {
        Pausar();
    }

    
    private void OnDisable()
    {
        Resumir();
    }

    public void Pausar()
    {
        uiMenuPausa.SetActive(true);
        Time.timeScale = 0f;
        AudioListener.pause = true;
        estaPausado = true;
        controlGrabacion.enabled = false;
        controlClima.enabled = false;
        controlVehiculo.enabled = false;
    }

    public void Resumir()
    {
        uiMenuPausa.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;
        estaPausado = false;
        controlGrabacion.enabled = true;
        controlClima.enabled = true;
        controlVehiculo.enabled = true;
    }


    public void MenuPrincipal()
    {
        condiciones.SetActive(false);
        ejercicios.SetActive(false);
        evaluacionActual.SetActive(false);
        princial.SetActive(true);
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
}
