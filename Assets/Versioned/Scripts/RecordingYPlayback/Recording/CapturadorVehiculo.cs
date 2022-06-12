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
        RecordingManager.Instance.VehicleModelIdList.Add(modelo);
        numeroVehiculo++;
    }
    private void OnEnable()
    {
        RecordingManager.Instance.OnCaptureSnapshot += CargarSnapshot;
    }
    void OnDisable()
    {
        RecordingManager.Instance.OnCaptureSnapshot -= CargarSnapshot;
    }
    
    public void CargarSnapshot(Recorder recorder)
    {
        SnapshotVehiculo snap = new SnapshotVehiculo();
        snap._transform = transform;   
        snap.ruedaFL = ruedaFL;
        snap.ruedaFR = ruedaFR;
        snap.ruedaRL = ruedaRL;
        snap.ruedaRR = ruedaRR;
        
        recorder.capturasVehiculos.Add(id, snap);
    }


}
