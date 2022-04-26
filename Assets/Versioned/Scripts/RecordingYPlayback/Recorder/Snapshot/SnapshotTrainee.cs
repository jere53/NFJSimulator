using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapshotTrainee : Snapshot
{
    public Transform _transform;
    public Transform ruedaFR;
    public Transform ruedaFL;
    public Transform ruedaRR;
    public Transform ruedaRL;
    public Transform volante;
    public bool estadoSirena;

    
    public PosicionYRotacion posYrot;
    public PosicionYRotacion posYrotRuedaFR;
    public PosicionYRotacion posYrotRuedaFL;
    public PosicionYRotacion posYrotRuedaRR;
    public PosicionYRotacion posYrotRuedaRL;
    public PosicionYRotacion posYrotVolante;
    
    public SnapshotTrainee(Transform transform, Transform ruedaFr, Transform ruedaFl, Transform ruedaRr, Transform ruedaRl, Transform volante)
    {
        _transform = transform;
        ruedaFR = ruedaFr;
        ruedaFL = ruedaFl;
        ruedaRR = ruedaRr;
        ruedaRL = ruedaRl;
        this.volante = volante;
    }
    public SnapshotTrainee()
    {
        
    }

    // public override string ToString()
    // {
    //     return "posicion: " + _transform.position;
    // }
}
