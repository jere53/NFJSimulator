using UnityEngine;

public class CapturadorSemaforo : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private Semaforo _semaforo;
    
    private void OnEnable()
    {
        Recorder.instancia.cantidadSemaforos++;
        Recorder.instancia.OnCapture += CargarSnapshot;
    }
    void OnDisable()
    {
        Recorder.instancia.cantidadSemaforos--;
        Recorder.instancia.OnCapture -= CargarSnapshot;
    }
    public void CargarSnapshot()
    {
        Recorder.instancia.colorSemaforos.Add(id, _semaforo.luzActual);
    }
}