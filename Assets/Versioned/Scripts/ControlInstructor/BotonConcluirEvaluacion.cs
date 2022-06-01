using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotonConcluirEvaluacion : MonoBehaviour
{
    public LocalSimulationController controller;

    public void ConcluirEvalcion()
    {
        controller.ConcluirEvaluacion();
    }
}
