using System.Collections;
using System.Collections.Generic;
using Trafico;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

public class AgentPool : MonoBehaviour
{
    [SerializeField] private List<GameObject> peatonPrefab;
    [SerializeField] private List<GameObject> vehiculoPrefab;

    private int indP = 0; //para que no spawnee siempre el mismo, va cambiando cual prefab genera secuencialmente.
    private int indV = 0;
    
    private Stack<Vehiculo> poolVehiculos = new Stack<Vehiculo>();
    private Stack<Peaton> poolPeatones = new Stack<Peaton>();

    public Peaton AdquirirPeaton()
    {
        Peaton devolucion;
        if (!poolPeatones.TryPop(out devolucion))
        {
            devolucion = Instantiate(peatonPrefab[indP]).GetComponent<Peaton>();
            devolucion.gameObject.GetComponent<NetworkObject>().Spawn(false);
            devolucion.gameObject.SetActive(false);
            indP = (indP + 1) % peatonPrefab.Count;
        }

        return devolucion;
    }

    public void LiberearPeaton(Peaton p)
    {
        p.gameObject.SetActive(false);
        poolPeatones.Push(p);
    }


    public Vehiculo AdquirirVehiculo()
    {
        Vehiculo devolucion;
        
        if (!poolVehiculos.TryPop(out devolucion))
        {
            devolucion = Instantiate(vehiculoPrefab[indV]).GetComponent<Vehiculo>();
            devolucion.gameObject.GetComponent<NetworkObject>().Spawn(false);
            devolucion.gameObject.SetActive(false);
            indV = (indV + 1) % vehiculoPrefab.Count;
        }

        return devolucion;
    }

    public void LiberearVehiculo(Vehiculo v)
    {
        v.gameObject.SetActive(false);
        poolVehiculos.Push(v);
    }
}
