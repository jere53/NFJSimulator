using System;
using System.Collections;
using System.Collections.Generic;
using Trafico;
using Unity.Collections;
using Unity.Jobs;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class SpawnManager : NetworkBehaviour
{
    public bool cambioMaximoPeatones; //solo para testear el cambio de maximo desde el editor
    public bool cambioMaximoVehiculos;
    
    public List<Peaton> peatonesHabilitados;

    public List<Vehiculo> vehiculosHabilitados;

    public float radioSpawneo = 200f; //el radio alrededor del trainee en el cual se pueden spawnear agentes
    
    private int peatonesASpawnear;

    private int vehiculosASpawnear;
    
    [SerializeField] private AgentPool spawneablesPool;
    [SerializeField] private VehiclePhysics.VehicleBase vehiculoTrainee;

    public int maxPeatones = 20;
    public int maxVehiculos = 20;


    private Collider[] vehicleSpawnerColliders = new Collider[20]; //optimizacion. Para poder usar el OverlapSphereNonAlloc
                                                                   //sin crear un nuevo arreglo cada vez que se invoca
    
    public IEnumerator SpawnearPeatonesInicial()
    {
        for (int i = 0; i < maxPeatones; i++)
        {
            var p = spawneablesPool.AdquirirPeaton();
            p.Spawn(vehiculoTrainee.transform.position, radioSpawneo, this);
            yield return null;
        }
        
    }

    public IEnumerator SpawnearVehiculosInicial()
    {
        Collider[] spawners = Physics.OverlapSphere(vehiculoTrainee.transform.position, 20000f,
            1 << 14, QueryTriggerInteraction.Collide);
        //conseguimos Spawners en rango de la ambulancia, el 1<<14 dice que solo tomemos collideres
        //correspondientes a la capa de fisica 14 que es la capa en la que definimos que estan los colliders de
        //los spawners. Aclaramos que si considere colliders Trigger porque son los que tienen los spawners de vehiculos
        //Aca no usamos el nonalloc porque solo se invoca una vez, ademas si siempre usamos el mismo arreglo puede haber
        //conflictos con otras corrutinas (ej, la corrutina de respawnear modifca el arreglo mientras el SpawnInicial esta
        //usandolo
        int spawnersLength = spawners.Length;
        if (spawnersLength > 0)
        {
            for (int i = 0; i < maxVehiculos; i++)
            {
                var v = spawneablesPool.AdquirirVehiculo();
                spawners[i % spawnersLength].GetComponent<SpawnerVehiculos>().Spawn(v, this);
                yield return null;
            }
        }
        else
        {
            Debug.Log("No hay spawners cerca del trainee, abortando spawning..." + " radio buscado: " + radioSpawneo);
            yield return null;
        }

       
    }

    public IEnumerator DeSpawnearPeatonesLejanosJugador()
    {
        while (true) //que se invoque permanentemente.
        {
            for (int i = 0; i < peatonesHabilitados.Count; i++)
            {
                if (Vector3.Distance(peatonesHabilitados[i].transform.position, vehiculoTrainee.transform.position) >
                    200f)
                {
                    spawneablesPool.LiberearPeaton(peatonesHabilitados[i]);
                    peatonesHabilitados.RemoveAt(i);
                    peatonesASpawnear++;
                    i--;
                }
                
                yield return null;
            }

            yield return new WaitForSeconds(4f);
        }
    }

    public IEnumerator DeSpawnearVehiculosLejanosJugador()
    {
        while (true)
        {
            for (int i = 0; i < vehiculosHabilitados.Count; i++)
            {
                if (Vector3.Distance(vehiculosHabilitados[i].transform.position, vehiculoTrainee.transform.position) >
                    10000f)
                {
                    spawneablesPool.LiberearVehiculo(vehiculosHabilitados[i]);
                    vehiculosHabilitados.RemoveAt(i);
                    vehiculosASpawnear++;
                    i--;
                    
                }
                
                yield return null;
            }

            yield return new WaitForSeconds(4f);
        }
    }
    
    //Intento fallido de aprovechar MultiThreading de Unity:
    /*
    public IEnumerator DeSpawnearPeatonesLejanosJugador()
    {
        while (true) //que se invoque permanentemente.
        {
            int j = peatonesHabilitados.Count; //peatones habilitados al momento de DeSpawnear
            
            NativeArray<float> a = new NativeArray<float>(j * 3, Allocator.TempJob); //posiciones de cada Agente

            NativeArray<float> b = new NativeArray<float>(3, Allocator.TempJob); //posicion del trainee

            NativeArray<float> result = new NativeArray<float>(j, Allocator.TempJob); //resultados para cada agente

            Transform vehiculoTraineeTransform = vehiculoTrainee.transform;
            //Vector3 posicionTrainee = vehiculoTraineeTransform.position; //problema: Esto no cambia, entonces puede
            //quedar desactualizado. Si el vehiculo se esta moviendo rapido, puede ser que se calcule que un peaton que
            //esta a 300 metros del vehiculo, el vehiculo se le acerca hasta que esta a 10m, y despues termina esta funcion
            //y dice "esta a 300 metros, matalo" y desaparece justo adelante del jugador. 
            //"Solucion", miramos hacia donde esta moviendose el trainee, y calculamos la distancia de los agentes
            //a un punto que este en esa direccion.
            
            //tampoco funciona.

            Vector3 destinoAproxTrainee = vehiculoTraineeTransform.forward + (vehiculoTrainee.localAcceleration * (vehiculoTrainee.speed));

            b[0] = vehiculoTraineeTransform.position.x;
            b[1] = vehiculoTraineeTransform.position.y;
            b[2] = vehiculoTraineeTransform.position.z;
            
            Vector3 posicionAgenteActual = new Vector3();
            
            for (int i = 0; i < j; i++)
            {
                int c = i * 3;
                posicionAgenteActual = peatonesHabilitados[i].transform.position;
                a[c] = posicionAgenteActual.x;
                a[c + 1] = posicionAgenteActual.y;
                a[c + 2] = posicionAgenteActual.z;

            }

            CalcularDistanciasParallelForJob distaincias = new CalcularDistanciasParallelForJob();

            distaincias.posicionesA = a;
            distaincias.posicionB = b;
            distaincias.result = result;

            JobHandle handle = distaincias.Schedule(result.Length, 1);
            
            handle.Complete(); //calculamos las distancias en paralelo

            a.Dispose();
            b.Dispose();
            
            for (int i = 0; i < j; i++)
            {
                if (result[i] > 400f)
                {
                    spawneablesPool.LiberearPeaton(peatonesHabilitados[i]);
                    peatonesHabilitados.RemoveAt(i);
                    peatonesASpawnear++;
                    i--;
                    j--;
                }
            }

            result.Dispose();

            yield return new WaitForSeconds(6f);
        }
    }
    
*/
    
    public IEnumerator ReSpawnearPeatones()
    {
        while (true)
        {
            //spawneamos de a 2
            while (peatonesASpawnear < 2) yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < 2; i++)
            {
                var p = spawneablesPool.AdquirirPeaton();
                p.Spawn(vehiculoTrainee.transform.position, radioSpawneo, this);
                peatonesASpawnear--;
            }

            yield return null;
        }
    }

    public IEnumerator ReSpawnearVehiculos()
    {
        while (true)
        {
            while (vehiculosASpawnear < 4) yield return new WaitForSeconds(0.5f);
            int spawnersEnRango = Physics.OverlapSphereNonAlloc(vehiculoTrainee.transform.position, 20000f,
                vehicleSpawnerColliders, 1 << 14, QueryTriggerInteraction.Collide);
            if (spawnersEnRango > 0)
            {
                for (int i = 0; i < vehiculosASpawnear; i++)
                {
                    var v = spawneablesPool.AdquirirVehiculo();
                    vehicleSpawnerColliders[i % spawnersEnRango].GetComponent<SpawnerVehiculos>().Spawn(v, this);
                    vehiculosASpawnear--;
                    yield return null; 
                    // tiene mas setido volver aca, para que no use los mismos spawners todo el tiempo
                    //si le ponemos que spawnee hasta 4, por ejemplo, siempre va a usar los primeros 4 colliders.
                }
            } else Debug.Log("No hay spawners cerca del trainee, abortando spawning..." + " radio buscado: " + radioSpawneo);

            yield return null;
        }
    }

    public IEnumerator CambioDeMaximoPeatones()
    {
        yield return new WaitForNextFrameUnit(); //Para que terminen de ejecutarse las corrutinas que interrumpimos en el ModificarMaximoPeatones
        int i = peatonesHabilitados.Count - 1;
        while (i > maxPeatones)
        {
            spawneablesPool.LiberearPeaton(peatonesHabilitados[i]);
            peatonesHabilitados.RemoveAt(i);
            i--;
        }

        peatonesASpawnear = maxPeatones - peatonesHabilitados.Count; //la nueva cantidad de peatones a spawnear. 
    }
    
    public IEnumerator CambioDeMaximoVehiculos()
    {
        yield return new WaitForNextFrameUnit();
        int i = vehiculosHabilitados.Count - 1;
        while (i > maxVehiculos)
        {
            spawneablesPool.LiberearVehiculo(vehiculosHabilitados[i]);
            vehiculosHabilitados.RemoveAt(i);
            i--;
        }

        vehiculosASpawnear = maxVehiculos - vehiculosHabilitados.Count;
    }
    
    public void ModificarMaximoPeatones(int nuevoMax)
    {
        maxPeatones = nuevoMax;
        StopCoroutine(DeSpawnearPeatonesLejanosJugador());
        StopCoroutine(ReSpawnearPeatones());
        //para que no haya comportamiento inesperado si se estan ejecutando otros spawneos/respawneos
        //Esto le dice a Unity que deje de re-invocarlas, pero no interrumpe la corrutina si ya se esta ejecutando este frame. 
        //entonces, en la corrutina cambio de maximo, esperamos al proximo frame para asegurarnos que ya terminaron de ejecutarse.
        
        StartCoroutine(CambioDeMaximoPeatones()); // esperamos al proximo frame y de-spawneamos peatones si el maximo se redujo.
        //calculamos la nueva cantidad de peatones a spawnear
        
        //volvemos a realizar el re-spawneo periodico.
        StartCoroutine(DeSpawnearPeatonesLejanosJugador());
        StartCoroutine(ReSpawnearPeatones());

    }
    
    [ServerRpc(RequireOwnership = false)]
    public void ModificarMaximoPeatonesServerRpc(int nuevoMax)
    {
        ModificarMaximoPeatones(nuevoMax);
    }

    public void ModificarMaximoVehiculos(int nuevoMax)
    {
        maxVehiculos = nuevoMax;
        StopCoroutine(DeSpawnearVehiculosLejanosJugador());
        StopCoroutine(ReSpawnearVehiculos());
        //para que no haya comportamiento inesperado si se estan ejecutando otros spawneos/respawneos
        //Esto le dice a Unity que deje de re-invocarlas, pero no interrumpe la corrutina si ya se esta ejecutando este frame. 
        //entonces, en la corrutina cambio de maximo, esperamos al proximo frame para asegurarnos que ya terminaron de ejecutarse.
        
        StartCoroutine(CambioDeMaximoVehiculos()); // esperamos al proximo frame y de-spawneamos peatones si el maximo se redujo.
        //calculamos la nueva cantidad de peatones a spawnear
        
        //volvemos a realizar el re-spawneo periodico.
        StartCoroutine(DeSpawnearVehiculosLejanosJugador());
        StartCoroutine(ReSpawnearVehiculos());

    }
    
    [ServerRpc(RequireOwnership = false)]
    public void ModificarMaximoVehiculosServerRpc(int nuevoMax)
    {
        ModificarMaximoVehiculos(nuevoMax);
    }
    
    
    
    public void ReSpawnPeaton(Peaton p)
    {
        peatonesHabilitados.Remove(p);
        spawneablesPool.LiberearPeaton(p);
        SpawnPeaton(); //commo saque uno de la lista, debo agregar otro para mantener la cantidad
    }

    public void ReSpawnVehiculo(Vehiculo v)
    {
        vehiculosHabilitados.Remove(v);
        spawneablesPool.LiberearVehiculo(v);
        SpawnVehiculo();
    }
    
    public void SpawnPeaton()
    {
        var p = spawneablesPool.AdquirirPeaton();
        p.Spawn(vehiculoTrainee.transform.position, radioSpawneo, this);
    }

    public void SpawnVehiculo()
    {
        var v = spawneablesPool.AdquirirVehiculo();
        
        Collider[] spawners = new Collider[1];
        Physics.OverlapSphereNonAlloc(vehiculoTrainee.transform.position, radioSpawneo,
            spawners, 1 << 14, QueryTriggerInteraction.Collide);

        if (spawners[0] == null)
        {
            Debug.Log("No hay spawners cerca del trainee, abortando spawning..." + " radio buscado: " + radioSpawneo);
            return;
        }
        
        spawners[0].GetComponent<SpawnerVehiculos>().Spawn(v, this);
    }

    public void HandleSpawnVehiculosYPeatones()
    {
        Debug.Log("Spawneando vehiculos y peatones");
        
        // Check if is host
        StartCoroutine(SpawnearPeatonesInicial()); //se spawnea el maximo seleccionado
        StartCoroutine(SpawnearVehiculosInicial());
            
        //se comienza a revisar si hay que re-spawnear peatones
        StartCoroutine(DeSpawnearPeatonesLejanosJugador());
        StartCoroutine(ReSpawnearPeatones());
        StartCoroutine(DeSpawnearVehiculosLejanosJugador());
        StartCoroutine(ReSpawnearVehiculos());
    }
    public void Update()
    {
        if (cambioMaximoPeatones)
        {
            ModificarMaximoPeatones(maxPeatones);
            cambioMaximoPeatones = false;
        }

        if (cambioMaximoVehiculos)
        {
            ModificarMaximoVehiculos(maxVehiculos);
            cambioMaximoVehiculos = false;
        }
    }
}
