using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehiclePhysics;

public class CriterioNafta : MonoBehaviour, ICriterio
{
    public float objetivoLitrosConsumidos;
    private float _litrosConsumidos;

    private VehicleBase _vehicle;

    private void Awake()
    {
        _vehicle = GetComponent<VehicleBase>();

        if (!_vehicle)
        {
            Debug.LogError("No se encontro un VPVehiclBase en el G.O. correspondiente al vehiculo del trainee");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //consumo de combustible actual en gramos/segundo
        float consumoInstantaneo = _vehicle.data.Get(Channel.Vehicle, VehicleData.EngineFuelRate) / 1000.0f;

        consumoInstantaneo /= 740; //para pasarlo a litros. El consumo esta en gramos/segundo, un litro de nafta son
        //approx 740 gramos, por lo tanto dividimos por 740
        
        if (consumoInstantaneo > 0)
            _litrosConsumidos += consumoInstantaneo * Time.deltaTime;
    }

    public void ObtenerDatosEvaluacion(ref DatosEvaluacion datosEvaluacion)
    {
        datosEvaluacion.DatosCriterioNafta = new Tuple<float, float>(_litrosConsumidos, objetivoLitrosConsumidos);
    }

    public void ComenzarEvaluacion()
    {
        _litrosConsumidos = 0;
        enabled = true;
        
    }

    public void ConcluirEvaluacion()
    {
        enabled = false;
    }
    
    public void Remover()
    {
        Destroy(this);
    }
}
