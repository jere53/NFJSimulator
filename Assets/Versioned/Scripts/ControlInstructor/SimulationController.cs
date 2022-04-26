using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class SimulationController : NetworkBehaviour
{
    public WeatherController weatherController;
    public SpawnManager spawnManager;
    public DayNightCicle dayNightCicle;
    public GameObject vehiculoTrainee;

    public List<ObjetivoLevantarPaciente> ejercicios; //Deberia haber un objetivo generico

    [ServerRpc(RequireOwnership = false)]
    public void SetWeatherServerRpc(string selectedPreset)
    {
        switch (selectedPreset)
        {
            case "sunny":
                weatherController.SunnyPreset();
                break;
            case "rainy":
                weatherController.RainPreset();
                break;
            case "cloudy":
                weatherController.CloudyPreset();
                break;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetVehiculosServerRpc(int maxVehiculos)
    {
        spawnManager.ModificarMaximoVehiculos(maxVehiculos);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPeatonesServerRpc(int maxPeatones)
    {
        spawnManager.ModificarMaximoPeatones(maxPeatones);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetToDServerRpc(float hora, float velocidad)
    {
        dayNightCicle.CambiarToD(hora, velocidad);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ComenzarEjercicioServerRpc(int indiceEjercicio, bool evalTiempo, int maxMinutos, int maxSegundos,
        bool evalCombustible, int maxLitros, bool evalVelocidad, int maxVelocidad, bool evalSemaforos, bool evalAccidentes)
    {
        if (indiceEjercicio < 0 || ejercicios.Count == 0) return;
        if (evalTiempo)
        {
            var cT = vehiculoTrainee.AddComponent<CriterioTiempo>();
            cT.minutosMaximosPermitidos = maxMinutos;
            cT.segundosMaximosPermitidos = maxSegundos;
        }

        if (evalCombustible)
        {
            var cC = vehiculoTrainee.AddComponent<CriterioNafta>();
            cC.objetivoLitrosConsumidos = maxLitros;
        }

        if (evalVelocidad)
        {
            var cV = vehiculoTrainee.AddComponent<CriterioVelocidadMaxima>();
            cV.velocidadMaximaKMh = maxVelocidad;
        }

        if (evalSemaforos)
        {
            vehiculoTrainee.AddComponent<CriterioRespetarSemaforos>();
        }
        
        if (evalAccidentes)
        {
            vehiculoTrainee.AddComponent<CriterioEvitarAccidentes>();
        }

        ejercicios[indiceEjercicio].Comenzar();
    }
    
    [ClientRpc]
    public void NotificarCambioExitosoClientRpc()
    {
        Debug.Log("CambioExitoso!");
    }
    
}
