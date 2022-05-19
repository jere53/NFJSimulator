using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class BotonAlmacenarEvaluacion : MonoBehaviour
{

    public LocalSimulationController controller;
    public TMP_InputField nombreArchivo;

    public void Almacenar()
    {
        string n = nombreArchivo.text;
        if (n.Equals(String.Empty)) n = DateTime.Now.ToString("yyyy-MM-dd-hh-mm--ss");
        controller.AlmacenarUltimaEvaluacion(n);
    }
}
