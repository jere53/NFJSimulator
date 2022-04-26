using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaybackTrainee : MonoBehaviour
{

    public GameObject rainParticleSystem;
    
    public Transform[] ruedas;
    
    public EstructuraGrabacion estructuraGrabacion;

    public Sirena sirena;
    public void ComenzarAEscuchar()
    {
        estructuraGrabacion.OnPlayIntervalo += ActualizarSnapshot;
    }
    
    public void ActualizarSnapshot()
    {
        //Debug.Log("EVENTO");
        if (estructuraGrabacion.snapshotTraineeIntervalo == null)
        {
            gameObject.SetActive(false);
            return;
        }
        
        transform.position = estructuraGrabacion.snapshotTraineeIntervalo.posYrot.position;
        transform.eulerAngles = estructuraGrabacion.snapshotTraineeIntervalo.posYrot.eulerAngles;
        
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
}
