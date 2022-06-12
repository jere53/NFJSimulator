using UnityEngine;

public class CapturadorSemaforo : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private Semaforo semaforo;
    
    private void OnEnable()
    {
        RecordingManager.Instance.streetLightCount++;
        RecordingManager.Instance.OnCaptureSnapshot += CargarSnapshot;
    }
    void OnDisable()
    {
        RecordingManager.Instance.streetLightCount--;
        RecordingManager.Instance.OnCaptureSnapshot -= CargarSnapshot;
    }
    public void CargarSnapshot(Recorder recorder)
    {
        recorder.colorSemaforos.Add(id, semaforo.luzActual);
    }
}