using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlaybackPeaton : MonoBehaviour, IPlayback
{
    public static int numeroPeaton;
    public int id;
    private Animator _animator;

    private void Awake()
    {
        id = numeroPeaton;
        numeroPeaton++;
        _animator = GetComponent<Animator>();
    }

    public EstructuraGrabacion estructuraGrabacion;
    
    public void ComenzarAEscuchar()
    {
        estructuraGrabacion.OnPlayIntervalo += ActualizarSnapshot;
    }

    public void ActualizarSnapshot()
    {
        SnapshotPeaton snapshotPeaton = estructuraGrabacion.GetSnapshotPeaton(id);
        if (snapshotPeaton == null)
        {
            _animator.SetFloat("velocidad", 0);
            gameObject.SetActive(false);
            return;
        }

        transform.position = snapshotPeaton.posYrot.position;
        transform.eulerAngles = snapshotPeaton.posYrot.eulerAngles;
        
        _animator.SetFloat("velocidad", snapshotPeaton.velocidad);
        if (snapshotPeaton.estaMuerto)
        {
            _animator.SetTrigger("atropellado");
        }
        else
        {
            
        }
        
        gameObject.SetActive(true);

    }
}
