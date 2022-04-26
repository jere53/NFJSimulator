using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapshotVehiculo : Snapshot
{
    public Transform _transform;
    public Transform ruedaFR;
    public Transform ruedaFL;
    public Transform ruedaRR;
    public Transform ruedaRL;

    public PosicionYRotacion posYrot;
    public PosicionYRotacion posYrotRuedaFR;
    public PosicionYRotacion posYrotRuedaFL;
    public PosicionYRotacion posYrotRuedaRR;
    public PosicionYRotacion posYrotRuedaRL;

    
    public SnapshotVehiculo(Transform transform, Transform ruedaFr, Transform ruedaFl, Transform ruedaRr, Transform ruedaRl)
    {
        _transform = transform;
        ruedaFR = ruedaFr;
        ruedaFL = ruedaFl;
        ruedaRR = ruedaRr;
        ruedaRL = ruedaRl;
    }
    public SnapshotVehiculo()
    {
    }
    
    
}
