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
    
    public EstructuraGrabacion estructuraGrabacion;
    public Reproductor reproductor;
    
    private SnapshotPeaton actual;
    private SnapshotPeaton siguiente;
    
    private float timeElapsed = 0;
    private float deltaIntervalo = 0;
    
    private void Update()
    {
        if (timeElapsed < deltaIntervalo)
        {
            transform.position = Vector3.Lerp(actual.posYrot.position, siguiente.posYrot.position, timeElapsed / deltaIntervalo);
            transform.eulerAngles = Vector3.Lerp(actual.posYrot.eulerAngles, siguiente.posYrot.eulerAngles, timeElapsed / deltaIntervalo);
            timeElapsed += Time.deltaTime;
        }
    }

    private void Awake()
    {
        id = numeroPeaton;
        numeroPeaton++;
        _animator = GetComponent<Animator>();
    }
    
    public void ComenzarAEscuchar()
    {
        //estructuraGrabacion.OnPlayIntervalo += ActualizarSnapshot;
        reproductor.OnPlayIntervalo += ActualizarSnapshot;
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

        if (reproductor.interpolar)
        {
            if (siguiente is null)
            {
                siguiente = snapshotPeaton;
            }
            else
            {
                actual = siguiente;
                deltaIntervalo = reproductor.deltaIntervalosRecording;
                timeElapsed = 0;
                siguiente = snapshotPeaton;

                Vector3 rotacionActual = actual.posYrot.eulerAngles;
                Vector3 rotacionSiguiente = siguiente.posYrot.eulerAngles;
                acomodarRotacion(rotacionActual, rotacionSiguiente);
            }
        }
        else
        {
            transform.position = snapshotPeaton.posYrot.position;
            transform.eulerAngles = snapshotPeaton.posYrot.eulerAngles;
        }

        _animator.SetFloat("velocidad", snapshotPeaton.velocidad);
        if (snapshotPeaton.estaMuerto)
        {
            _animator.SetTrigger("atropellado");
        }

        gameObject.SetActive(true);

    }
    
    private void acomodarRotacion(Vector3 rotacionActual, Vector3 rotacionSiguiente)
    {
        //esto en el caso de que alguna de las coordenadas de la rotacion del actual este en un valor muy chico (por ejemplo 1) y 
        // el otro en un valor muy grande (por ejemplo 359). En este caso la interpolacion daria una vuelta completa, por lo que esto evita ese error
        if (Math.Abs(rotacionActual.x - rotacionSiguiente.x) > 180)
            if (rotacionActual.x > rotacionSiguiente.x)
                siguiente.posYrot.eulerAngles.x += 360;
            else
                actual.posYrot.eulerAngles.x += 360;    
            
        if (Math.Abs(rotacionActual.y - rotacionSiguiente.y) > 180)
            if (rotacionActual.y > rotacionSiguiente.y)
                siguiente.posYrot.eulerAngles.y += 360;
            else
                actual.posYrot.eulerAngles.y += 360;     
            
        if (Math.Abs(rotacionActual.z - rotacionSiguiente.z) > 180)
            if (rotacionActual.z > rotacionSiguiente.z)
                siguiente.posYrot.eulerAngles.z += 360;
            else
                actual.posYrot.eulerAngles.z += 360;
    }
}
