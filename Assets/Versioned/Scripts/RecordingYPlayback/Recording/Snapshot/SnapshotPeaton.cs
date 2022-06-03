using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapshotPeaton : Snapshot
{
    public Transform _transform;
    public float velocidad;
    
    public bool estaMuerto;

    public PosicionYRotacion posYrot;

    public SnapshotPeaton(Transform transform, float velocidad)
    {
        _transform = transform;
        this.velocidad = velocidad;
    }
    public SnapshotPeaton()
    {
    }

}
