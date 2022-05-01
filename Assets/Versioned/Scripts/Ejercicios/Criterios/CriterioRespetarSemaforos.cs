using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.MPE;
using UnityEngine;
using VehiclePhysics;

public class CriterioRespetarSemaforos : MonoBehaviour, ICriterio
{
    private bool _evaluando;
    private int _semaforosSalteados = 0;
    private int _cantidadDeColliders;
    private int _cantidadDeCollidersPasandoSemaforo;
    private bool _enSemaforo;
    private Vector3 posicionSemaforo;
    private Transform centroVehiculo;
    private Collider colliderSemaforo;
    private float _tiempoDesdeComienzo = 0;
    public List<float> momentosInfracciones = new List<float>();
    private float distancia;
    private VPVehicleController _vehicleController;

    private void Awake()
    {
        centroVehiculo = transform.Find("LimitesSemaforo");
        
        if (!centroVehiculo)
        {
            Debug.LogError("No se encontro la transform con el centro del vehiculo en el GameObject correspondiente al vehiculo del Trainee.");
        }

        _vehicleController = GetComponent<VPVehicleController>();
    }

    private void Update()
    {
        if (_evaluando)
        {
            _tiempoDesdeComienzo += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_evaluando) return;

        if (other.CompareTag("Semaforo"))
        {
            posicionSemaforo = other.bounds.center; //el centro del collider
            colliderSemaforo = other;
            if (!_enSemaforo)
            {
                _enSemaforo = true;
                StartCoroutine(DeterminarSalteoSemaforo());
            }
        }
    }

    IEnumerator DeterminarSalteoSemaforo()
    {
        //El collider se activa cuando el semaforo esta en amarillo. Cambia de amarillo a rojo/verde despues de 1 segundo.
        //Decimos que espere 1.2 segundos en caso de que el collider no se desactive exactamente un segundo despues.
        yield return new WaitForSeconds(1.2f);
        
        //Si el collider esta desactivado, significa que toco el collider mientras el semaforo estaba en amarillo cambiando
        //a verde, y no hay infraccion. Si esta activado, es que paso de amarillo a rojo.
        if (colliderSemaforo.enabled)
        {

            //El trainee podria haber pasado en amarillo
            //(esta a mas de 10  metros de donde tenia que frenar para cuando el semaforo pasa de amarillo a rojo)

            distancia = Vector3.Distance(centroVehiculo.position, posicionSemaforo);

            if (distancia < 10f)
            {
                //No llego a pasar en amarillo

                if (_vehicleController.data.Get(Channel.Vehicle, VehicleData.Speed) / 1000f * 3.6 < 15f
                    && _vehicleController.data.Get(Channel.Input, InputData.Throttle) / 10000f < 10f)
                {
                    //Si se mueve a menos de 15 kmh y no esta acelerando, probablemente solo se adelanto un poco
                    Debug.Log("Se esta adelantando demasiado");
                } 
                else
                {
                    momentosInfracciones.Add(_tiempoDesdeComienzo);
                    Debug.Log("SEMAFORO EN ROJO");
                }
            }
        }

        _enSemaforo = false;
    }
    
    public void PresentarEvaluacion()
    {
        string resultadoEvaluacion = "El Evaluado paso semaforos en rojo en los siguientes segundos luego de comenzada la evaluacion '\n'";
        foreach (var VARIABLE in momentosInfracciones)
        {
            resultadoEvaluacion += VARIABLE + '\n';
        }
        Debug.Log(resultadoEvaluacion);
    }

    public void ComenzarEvaluacion()
    {
        _tiempoDesdeComienzo = 0;
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
