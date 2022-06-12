using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaybackTrainee : MonoBehaviour
{

    public GameObject rainParticleSystem;
    public Transform[] ruedas;
    public Sirena sirena;
    
    public EstructuraGrabacion estructuraGrabacion;
    public Reproductor reproductor;

    private SnapshotTrainee actual;
    private SnapshotTrainee siguiente;

    private float timeElapsed = 0;
    private float deltaIntervalo = 0;

    private void Update()
    {
        if (timeElapsed < deltaIntervalo)
        {
            // Debug.Log("INTERPOLACION");
            // Debug.Log("tiempo update: " + Time.deltaTime);
            transform.position = Vector3.Lerp(actual.posYrot.position, siguiente.posYrot.position, timeElapsed / deltaIntervalo);
            transform.eulerAngles = Vector3.Lerp(actual.posYrot.eulerAngles, siguiente.posYrot.eulerAngles, timeElapsed / deltaIntervalo);
            // Debug.Log( "interpolado: " + Vector3.Lerp(actual.posYrot.eulerAngles, siguiente.posYrot.eulerAngles, timeElapsed / deltaIntervalo));
            timeElapsed += Time.deltaTime;
        }
    }

    public void ComenzarAEscuchar()
    {
        //estructuraGrabacion.OnPlayIntervalo += ActualizarSnapshot;
        reproductor.OnPlayIntervalo += ActualizarSnapshot;
    }
    
    public void ActualizarSnapshot()
    {
        //Si el objeto no está en ese frame, se debe desactivar
        if (estructuraGrabacion.snapshotTraineeIntervalo == null)
        {
            gameObject.SetActive(false);
            return;
        }

        if (reproductor.interpolar)
        {
            if (siguiente is null) //si es el primer frame, debo esperar hasta leer otro
            {
                siguiente = estructuraGrabacion.snapshotTraineeIntervalo;
            }
            else
            {
                actual = siguiente;
                deltaIntervalo = reproductor.deltaIntervalosRecording;
                timeElapsed = 0; //reseteo el valor para la interpolacion
                siguiente = estructuraGrabacion.snapshotTraineeIntervalo;

                Vector3 rotacionActual = actual.posYrot.eulerAngles;
                Vector3 rotacionSiguiente = siguiente.posYrot.eulerAngles; 
                AcomodarRotacion(rotacionActual, rotacionSiguiente);
            }
        }
        else
        {
            transform.position = estructuraGrabacion.snapshotTraineeIntervalo.posYrot.position;
            transform.eulerAngles = estructuraGrabacion.snapshotTraineeIntervalo.posYrot.eulerAngles;
        }
        
            
        ruedas[0].position = estructuraGrabacion.snapshotTraineeIntervalo.posYrotRuedaFL.position;
        ruedas[0].eulerAngles = estructuraGrabacion.snapshotTraineeIntervalo.posYrotRuedaFL.eulerAngles;
        
        ruedas[1].position = estructuraGrabacion.snapshotTraineeIntervalo.posYrotRuedaFR.position;
        ruedas[1].eulerAngles = estructuraGrabacion.snapshotTraineeIntervalo.posYrotRuedaFR.eulerAngles;
        
        ruedas[2].position = estructuraGrabacion.snapshotTraineeIntervalo.posYrotRuedaRL.position;
        ruedas[2].eulerAngles = estructuraGrabacion.snapshotTraineeIntervalo.posYrotRuedaRL.eulerAngles;
        
        ruedas[3].position = estructuraGrabacion.snapshotTraineeIntervalo.posYrotRuedaRR.position;
        ruedas[3].eulerAngles = estructuraGrabacion.snapshotTraineeIntervalo.posYrotRuedaRR.eulerAngles;
        
        if (estructuraGrabacion.snapshotTraineeIntervalo.estadoSirena)
        {
            sirena.Encender();
        }
        else
        {
            sirena.Apagar();
        }

        gameObject.SetActive(true);
        
    }

    private void AcomodarRotacion(Vector3 rotacionActual, Vector3 rotacionSiguiente)
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
