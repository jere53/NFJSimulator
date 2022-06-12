using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaybackSemaforo : MonoBehaviour, IPlayback
{
    public int id;
    private Semaforo _semaforo;

    public EstructuraGrabacion estructuraGrabacion;
    public Reproductor reproductor;
    
    private void Awake()
    {
        _semaforo = GetComponent<Semaforo>();
    }

    public void ComenzarAEscuchar()
    {
        reproductor.OnPlayIntervalo += ActualizarSnapshot;
    }


    public void ActualizarSnapshot()
    {
        int snapshotSemaforo = estructuraGrabacion.GetSnapshotSemaforo(id);
        if (snapshotSemaforo == -1)
        {
            Debug.LogWarning("Un semaforo no tiene snapshots grabados: " + gameObject.name);
            return;
        }

        switch (snapshotSemaforo)
        {
            case 0:
                _semaforo.RojoInmediato();
                break;
            case 1:
                _semaforo.AmarilloInmediato();
                break;
            case 2:
                _semaforo.VerdeInmediato();
                break;
        }
    }

    private void OnDisable()
    {
        estructuraGrabacion.OnPlayIntervalo -= ActualizarSnapshot;
    }
    
    
}