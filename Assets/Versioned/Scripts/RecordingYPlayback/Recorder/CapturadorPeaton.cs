using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CapturadorPeaton : MonoBehaviour
{
    [SerializeField] public int modelo;
    
    private static int numeroPeaton;
    private int id;
    private NavMeshAgent navMesh;
    private Animator _animator;
    private void Awake()
    {
        id = numeroPeaton;
        Recorder.instancia.modelosPeatones.Add(modelo);
        numeroPeaton++;
        navMesh = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
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
        SnapshotPeaton snap = new SnapshotPeaton();
        snap._transform = transform;
        snap.velocidad = navMesh.speed;
        bool estaMuerto = _animator.GetCurrentAnimatorStateInfo(0).IsName("Atropellado");
        snap.estaMuerto = estaMuerto;
        Recorder.instancia.capturasPeatones.Add(id, snap);
    }

}
