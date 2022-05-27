using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrieferModel : MonoBehaviour
{
    private DatosEvaluacion _datosEvaluacion;

    public void CargarDatosEvaluacion(string path)
    {
        _datosEvaluacion = DatosEvaluacion.CargarDeDisco(path);
        _datosEvaluacion.Presentar();
    }
}
