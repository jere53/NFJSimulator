using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehiclePhysics;

public class CriterioNafta : MonoBehaviour, ICriterio
{
    public float objetivoLitrosConsumidos;
    private float litrosConsumidos;
    private bool _evaluando;

    private VehicleBase _vehicle;

    // Update is called once per frame
    void Update()
    {
        if(!_evaluando) return;
        //consumo de combustible actual en gramos/segundo
        float consumoInstantaneo = _vehicle.data.Get(Channel.Vehicle, VehicleData.EngineFuelRate) / 1000.0f;

        consumoInstantaneo /= 740; //para pasarlo a litros. El consumo esta en gramos/segundo, un litro de nafta son
        //approx 740 gramos, por lo tanto dividimos por 740
        
        if (consumoInstantaneo > 0)
            litrosConsumidos += consumoInstantaneo * Time.deltaTime;
    }

    public void PresentarEvaluacion()
    {
        string resultado = "El objetivo de consumo de combustible eran: " + objetivoLitrosConsumidos + " litros."
                           + '\n' + "El Evaluado consumio " + litrosConsumidos + " litros";

        Debug.Log(resultado);
    }

    public void ComenzarEvaluacion()
    {
        _vehicle = GetComponent<VehicleBase>();
        _evaluando = true;
    }

    public void ConcluirEvaluacion()
    {
        _evaluando = false;
    }
    
    public void Remover()
    {
        Destroy(this);
    }
}
