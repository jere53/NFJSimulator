using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CapturadorTrainee : MonoBehaviour
{
    [SerializeField] private Transform ruedaFL;
    [SerializeField] private Transform ruedaFR;
    [SerializeField] private Transform ruedaRL;
    [SerializeField] private Transform ruedaRR;
    [SerializeField] private Transform volante;
    [SerializeField] private Sirena sirena;


    void OnEnable()
    {
        RecordingManager.Instance.OnCaptureSnapshot += CargarSnapshot;
    }    
    
    void OnDisable()
    {
        RecordingManager.Instance.OnCaptureSnapshot -= CargarSnapshot;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (RecordingManager.Instance.isRecording)
            {
                RecordingManager.Instance.StopRecording();
            }
            else
            {
                RecordingManager.Instance.StartRecording();
            }
        }
    }

    public void CargarSnapshot(Recorder recorder)
    {
        SnapshotTrainee snap = new SnapshotTrainee();
        snap._transform = transform;
        // snap.volante = volante;
        // snap.ruedaFL = ruedaFL;
        // snap.ruedaFR = ruedaFR;
        // snap.ruedaRL = ruedaRL;
        // snap.ruedaRR = ruedaRR;
        snap.estadoSirena = sirena.sirenaActiva;

        snap.ruedaFL = gameObject.transform.GetChild(0).GetChild(1).GetChild(1);
        snap.ruedaFR = gameObject.transform.GetChild(0).GetChild(2).GetChild(1);
        snap.ruedaRL = gameObject.transform.GetChild(0).GetChild(3).GetChild(1);
        snap.ruedaRR = gameObject.transform.GetChild(0).GetChild(4).GetChild(1);
        snap.volante = gameObject.transform.GetChild(0).GetChild(0).GetChild(13);
        recorder.capturaTrainee = snap;
    }

}