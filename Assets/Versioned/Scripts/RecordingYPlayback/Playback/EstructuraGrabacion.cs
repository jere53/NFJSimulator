using System;
using System.Collections.Generic;
using UnityEngine;

public class EstructuraGrabacion : MonoBehaviour
{
    public struct IntervaloClimaToD
    {
        public int clima;
        public float hora;
        public float velocidadOrbita;
    }
    public struct IntervaloGrabacion
    {
        public Dictionary<int, SnapshotVehiculo> snapshotVehiculos;
        public Dictionary<int, SnapshotPeaton> snapshotPeatones;
        public SnapshotTrainee snapshotTrainee;
        public Dictionary<int, int> snapshotSemaforo;
    }

    //public Dictionary<int, EstadoInicialInterseccion> estadosInicialesIntersecciones = new Dictionary<int, EstadoInicialInterseccion>();
    [NonSerialized]public WeatherController weatherController;
    [NonSerialized]public DayNightCicle dayNightCicle;
    public Queue<IntervaloClimaToD> grabacionClimaToD = new Queue<IntervaloClimaToD>();
    public Queue<IntervaloGrabacion> grabacion = new Queue<IntervaloGrabacion>();
    public Dictionary<int, SnapshotVehiculo> snapshotVehiculosIntervalo;
    public Dictionary<int, SnapshotPeaton> snapshotPeatonesIntervalo;
    public SnapshotTrainee snapshotTraineeIntervalo;
    public Dictionary<int, int> snapshotSemaforoIntervalo;
    public List<DatosEvaluacion> evals;
    
    public delegate void ActualizarSnapshot();
    public event ActualizarSnapshot OnPlayIntervalo;

    public int numeroIntervalo;

    public SnapshotTrainee GetSnapshotTrainee()
    {
        return snapshotTraineeIntervalo;
    }

    public SnapshotVehiculo GetSnapshotVehiculo(int id)
    {
        return snapshotVehiculosIntervalo.GetValueOrDefault(id, null);
    }
    
    public SnapshotPeaton GetSnapshotPeaton(int id)
    {
        return snapshotPeatonesIntervalo.GetValueOrDefault(id, null);
    }

    public int GetSnapshotSemaforo(int id)
    {
        return snapshotSemaforoIntervalo.GetValueOrDefault(id, -1);
    }
    
    public void PlayIntervalo(int intervalo)
    {
        IntervaloGrabacion intervaloActual = grabacion.Dequeue();
        
        snapshotPeatonesIntervalo = intervaloActual.snapshotPeatones;
        snapshotTraineeIntervalo = intervaloActual.snapshotTrainee;
        snapshotVehiculosIntervalo = intervaloActual.snapshotVehiculos;
        snapshotSemaforoIntervalo = intervaloActual.snapshotSemaforo;
        
        numeroIntervalo = intervalo;
        
        OnPlayIntervalo?.Invoke();
    }

    public void PlayIntervaloClimaToD()
    {
        IntervaloClimaToD intervaloClimaToDActual = grabacionClimaToD.Dequeue();
        switch (intervaloClimaToDActual.clima)
        {
            case 0:
                weatherController.SunnyPreset();
                break;
            case 1:
                weatherController.RainPreset();
                break;
            case 2:
                weatherController.CloudyPreset();
                break;
        }
        dayNightCicle.orbitSpeed = intervaloClimaToDActual.velocidadOrbita;
        dayNightCicle.timeOfDay = intervaloClimaToDActual.hora;
    }

    public void PlayIntervaloEval(int intervalo)
    {
        evals[intervalo].Presentar();
    }
}
