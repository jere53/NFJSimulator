using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BotonCargarEvaluacion : MonoBehaviour
{
    public LocalSimulationController controller;
    public TMP_InputField nombreArchivo;
    public void CargarEvaluacion()
    {
        controller.CargarEvaluacionTest(nombreArchivo.text);
    }
}
