using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SelectorConexion : MonoBehaviour
{
    public GameObject rootSimulacion;
    public GameObject rootInterfazInstructor;
    public GameObject interfazSeleccion;
    
    public void StartTrainee()
    {
        NetworkManager.Singleton.StartServer();
        interfazSeleccion.SetActive(false);
        rootSimulacion.SetActive(true);
    }

    public void StartInstructor()
    {
        NetworkManager.Singleton.StartClient();
        interfazSeleccion.SetActive(false);
        rootInterfazInstructor.SetActive(true);
    }
}
