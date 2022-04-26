using UnityEngine;

public class PlaybackVehiculo : MonoBehaviour
{
    public static int numeroVehiculo;
    public int id;

    private void Awake()
    {
        id = numeroVehiculo;
        numeroVehiculo++;
    }

    public EstructuraGrabacion estructuraGrabacion;
    
    public void ComenzarAEscuchar()
    {
        estructuraGrabacion.OnPlayIntervalo += ActualizarSnapshot;
    }

    public void ActualizarSnapshot()
    {
        SnapshotVehiculo snapshotVehiculo = estructuraGrabacion.GetSnapshotVehiculo(id);
        if (snapshotVehiculo == null)
        {
            gameObject.SetActive(false);
            return;
        }
        
        transform.position = snapshotVehiculo.posYrot.position;
        transform.eulerAngles = snapshotVehiculo.posYrot.eulerAngles;
        
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
}
