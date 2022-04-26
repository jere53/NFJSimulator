using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehiclePhysics;

public class CriterioVelocidadMaxima : MonoBehaviour, ICriterio
{

    public float velocidadMaximaKMh;

    private VehicleBase _vehicle;
    
    private List<string> _excesosDeVelocidad = new List<string>();
    
    private float _tiempoActual;

    
    // Start is called before the first frame update
    private void Update()
    {
        _tiempoActual += Time.deltaTime;
    }

    // Revisa la velocidad periodicamente
    IEnumerator RevisarVelocidad()
    {
        while (true)
        {
            float velocidadActual =
                (_vehicle.data.Get(Channel.Vehicle, VehicleData.Speed) / 1000f) * 3.6f; //para llevarlo a KM/h 

            if (velocidadActual > velocidadMaximaKMh)
            {
                TimeSpan tiempoInfraccion = TimeSpan.FromSeconds(_tiempoActual);
                _excesosDeVelocidad.Add("Exceso de velocidad a los " + tiempoInfraccion.Minutes + "\'" +
                                       tiempoInfraccion.Seconds + "\""
                                       + " de comenzada la evaluacion" + '\n' + "Velocidad Maxima: " 
                                       + velocidadMaximaKMh + " velocidad del Evaluado: " + velocidadActual);
                yield return new WaitForSeconds(10f); //para darle tiempo a que baje la velocidad.
            }

            yield return new WaitForSeconds(2f); 
        }
    }

    public void PresentarEvaluacion()
    {
        foreach (var s in _excesosDeVelocidad)
        {
            Debug.Log(s);
        }
    }

    public void ComenzarEvaluacion()
    {
        _vehicle = GetComponent<VehicleBase>();
        StartCoroutine(RevisarVelocidad());
    }

    public void ConcluirEvaluacion()
    {
        StopCoroutine(RevisarVelocidad());
    }
    
    public void Remover()
    {
        Destroy(this);
    }
}
