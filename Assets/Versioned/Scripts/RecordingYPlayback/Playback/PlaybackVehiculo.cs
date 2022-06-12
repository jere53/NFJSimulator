using System;
using UnityEngine;

public class PlaybackVehiculo : MonoBehaviour
{
    public static int numeroVehiculo;
    public int id;

    public EstructuraGrabacion estructuraGrabacion;
    public Reproductor reproductor;

    private SnapshotVehiculo actual;
    private SnapshotVehiculo siguiente;
    
    private float timeElapsed = 0;
    private float deltaIntervalo = 0;
    
    private void Awake()
    {
        id = numeroVehiculo;
        numeroVehiculo++;
    }
    
    private void Update()
    {
        if (timeElapsed < deltaIntervalo)
        {
            transform.position = Vector3.Lerp(actual.posYrot.position, siguiente.posYrot.position, timeElapsed / deltaIntervalo);
            transform.eulerAngles = Vector3.Lerp(actual.posYrot.eulerAngles, siguiente.posYrot.eulerAngles, timeElapsed / deltaIntervalo);
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
        SnapshotVehiculo snapshotVehiculo = estructuraGrabacion.GetSnapshotVehiculo(id);
        if (snapshotVehiculo == null)
        {
            gameObject.SetActive(false);
            return;
        }

        if (reproductor.interpolar)
        {
            if (siguiente is null)
            {
                siguiente = snapshotVehiculo;
            }
            else
            {
                actual = siguiente;
                deltaIntervalo = reproductor.deltaIntervalosRecording;
                timeElapsed = 0;
                siguiente = snapshotVehiculo;

                Vector3 rotacionActual = actual.posYrot.eulerAngles;
                Vector3 rotacionSiguiente = siguiente.posYrot.eulerAngles;

                acomodarRotacion(rotacionActual, rotacionSiguiente);
            }
        }
        else
        {
            transform.position = snapshotVehiculo.posYrot.position;
            transform.eulerAngles = snapshotVehiculo.posYrot.eulerAngles;
        }

        /*
        ruedas[0].position = snapshotVehiculo.posYrotRuedaFL.position;
        ruedas[0].eulerAngles = snapshotVehiculo.posYrotRuedaFL.eulerAngles;
        
        ruedas[1].position = snapshotVehiculo.posYrotRuedaFR.position;
        ruedas[1].eulerAngles = snapshotVehiculo.posYrotRuedaFR.eulerAngles;
        
        ruedas[2].position = snapshotVehiculo.posYrotRuedaRL.position;
        ruedas[2].eulerAngles = snapshotVehiculo.posYrotRuedaRL.eulerAngles;
        
        ruedas[3].position = snapshotVehiculo.posYrotRuedaRR.position;
        ruedas[3].eulerAngles = snapshotVehiculo.posYrotRuedaRR.eulerAngles;
        */
        
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
