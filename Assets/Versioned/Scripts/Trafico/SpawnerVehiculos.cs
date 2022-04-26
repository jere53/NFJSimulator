using System.Collections;
using System.Collections.Generic;
using Trafico;
using UnityEngine;

public class SpawnerVehiculos : MonoBehaviour
{
    public Waypoint waypointInicial;

    private SpawnManager _spawnManager;
    private Queue<Vehiculo> colaDeSpawning = new Queue<Vehiculo>();

    bool HayEspacio()
    {
        //Si hay objetos fisicos que sean peatones, otros autos, o el trainee (capas de fisica 13, 12, 8, respectivamente)
        return !Physics.CheckSphere(transform.position, 2.5f, (1 << 13) | (1 << 12) | (1 << 8));
    }

    public void Spawn(Vehiculo v, SpawnManager spawnManager)
    {
        if (HayEspacio())
        {
            _spawnManager = spawnManager;
            v.Spawn(transform.position, waypointInicial, _spawnManager);
        }
        else
        {
            colaDeSpawning.Enqueue(v);
            StartCoroutine(SpawnPendientes());
        }
    }

    private IEnumerator SpawnPendientes()
    {
        while (colaDeSpawning.Count > 0)
        {
            yield return new WaitForSeconds(2f);
            if (_spawnManager.vehiculosHabilitados.Count == _spawnManager.maxVehiculos)
            {
                colaDeSpawning.Clear();
            } else if (HayEspacio())
            {
                Vehiculo v;
                colaDeSpawning.Dequeue().Spawn(transform.position, waypointInicial, _spawnManager);
                
            }
        }
    }
}
