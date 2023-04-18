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
    
    public Queue<IntervaloClimaToD> grabacionClimaToD = new Queue<IntervaloClimaToD>();
    public Queue<IntervaloGrabacion> grabacion = new Queue<IntervaloGrabacion>();
    public Dictionary<int, SnapshotVehiculo> snapshotVehiculosIntervalo;
    public Dictionary<int, SnapshotPeaton> snapshotPeatonesIntervalo;
    public SnapshotTrainee snapshotTraineeIntervalo;
    public Dictionary<int, int> snapshotSemaforoIntervalo;
    public List<DatosEvaluacion> evals;

    public IntervaloClimaToD intervaloClimaToDActual;
    
    public delegate void ActualizarSnapshot();
    public event ActualizarSnapshot OnPlayIntervalo;

    public int numeroIntervalo;

    public delegate void MostrarInfraccion(DatosEvaluacion datosEvaluacion);
    public event MostrarInfraccion OnMostrarInfraccion;
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
    
    public bool SiguienteIntervalo()
    {
        if (grabacion.Count == 0)
            return false;
        
        IntervaloGrabacion intervaloActual = grabacion.Dequeue();
        
        snapshotPeatonesIntervalo = intervaloActual.snapshotPeatones;
        snapshotTraineeIntervalo = intervaloActual.snapshotTrainee;
        snapshotVehiculosIntervalo = intervaloActual.snapshotVehiculos;
        snapshotSemaforoIntervalo = intervaloActual.snapshotSemaforo;
        return true;
    }

    public bool SiguienteIntervaloClimaToD()
    {
        Debug.Log("PlayIntervaloClimaToD");
        if (grabacionClimaToD.Count == 0)
            return false;
        intervaloClimaToDActual = grabacionClimaToD.Dequeue();
        return true;
    }
    
}
