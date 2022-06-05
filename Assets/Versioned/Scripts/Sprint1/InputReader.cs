using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehiclePhysics;

public class InputReader : MonoBehaviour
{

    public FallasVehiculo FallasVehiculo;
    public WeatherController WeatherController;
    public Sirena Sirena;
    public Recorder Recorder;
    public int fps;
    public int fpsClima;
    public int cont = 0; 
    

    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        //Clima
         // if (Input.GetMouseButtonDown(0)) {
         //     WeatherController.RainPreset();
         // }
         //
         // if (Input.GetMouseButtonDown(1))
         // {
         //     WeatherController.SunnyPreset();
         // }
         //
         // if (Input.GetMouseButtonDown(2))
         // {
         //     WeatherController.CloudyPreset();
         // }
        
         // Grabar en Recorder
         // if (Input.GetKeyDown(KeyCode.G))
         // {
         //     Recorder.GrabacionManager(fps, fpsClima);
         // }
         
         // //Sirena
         // if (Input.GetKeyDown(KeyCode.P))
         // {
         //     Sirena.ToggleSirena();
         // }
        
        //Fallas en vehiculo
        if (Input.GetKeyDown(KeyCode.L))
        {
            FallasVehiculo.Frenos();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            FallasVehiculo.FrenoDeManos();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            FallasVehiculo.PincharRueda(cont);
            cont++;
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            FallasVehiculo.EmpaniarVidrio();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            FallasVehiculo.DesempaniarVidrio();
        }
        
        
    }
}
