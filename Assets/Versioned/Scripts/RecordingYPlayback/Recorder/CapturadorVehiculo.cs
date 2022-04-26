using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturadorVehiculo : MonoBehaviour
{
    [SerializeField] public int modelo;
    [SerializeField] private Transform ruedaFL;
    [SerializeField] private Transform ruedaFR;
    [SerializeField] private Transform ruedaRL;
    [SerializeField] private Transform ruedaRR;
    
    public static int numeroVehiculo;
    public int id;
    private void Awake()
    {
        id = numeroVehiculo;
        Recorder.instancia.modelosVehiculos.Add(modelo);
        numeroVehiculo++;
    }
    private void OnEnable()
    {
        Recorder.instancia.OnCapture += CargarSnapshot;
    }
    void OnDisable()
    {
        Recorder.instancia.OnCapture -= CargarSnapshot;
    }
    
    

    public void CargarSnapshot()
    {
        SnapshotVehiculo snap = new SnapshotVehiculo();
        snap._transform = transform;   
        snap.ruedaFL = ruedaFL;
        snap.ruedaFR = ruedaFR;
        snap.ruedaRL = ruedaRL;
        snap.ruedaRR = ruedaRR;
        
        Recorder.instancia.capturasVehiculos.Add(id, snap);
    }


}
