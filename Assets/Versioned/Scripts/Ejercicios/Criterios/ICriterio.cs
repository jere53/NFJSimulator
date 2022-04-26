using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICriterio
{
    public void PresentarEvaluacion();

    public void ComenzarEvaluacion();

    public void ConcluirEvaluacion();

    public void Remover();
}
