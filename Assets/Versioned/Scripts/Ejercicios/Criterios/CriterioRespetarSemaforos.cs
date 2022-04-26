using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriterioRespetarSemaforos : MonoBehaviour, ICriterio
{
    private bool _evaluando;
    private int _semaforosSalteados = 0;
    private int _cantidadDeColliders;
    private int _cantidadDeCollidersPasandoSemaforo;
    private bool _enSemaforo;

    private void OnTriggerEnter(Collider other)
    {
        if (!_evaluando) return;

        if (other.CompareTag("Semaforo"))
        {
            _cantidadDeCollidersPasandoSemaforo++;
            if (!_enSemaforo)
            {
                _enSemaforo = true;
                StartCoroutine(DeterminarSalteoSemaforo());
            }
            
        }
    }

    IEnumerator DeterminarSalteoSemaforo()
    {
        yield return new WaitForSeconds(2f); 
        //esperamos para estar seguros que ocurrieron todas las colisiones con el semaforo
        //usamos 2 segundos porque el collider del semaforo mide approx 1 metro, por lo tanto para que no se detecte bien,
        //el vehiculo deberia pasar el semaforo en rojo a 0.5m/s, o 1.8 KM/h, lo cual no es factible.
        //Tratamos de usar un numero chico para que no considere 2 colisiones como una sola. Para que eso pasara, el
        //vehiculo tendria que pasar por 2 semaforos en menos de 2 segundos. Como hay un semaforo que puede pasar por
        //cuadra, tiene que recorrer 2 cuadras (aprox 200 metros) en 2 segundos, es decir, andar a 360 KM/h, lo cual
        //tampoco es factible.
        if (_cantidadDeCollidersPasandoSemaforo > _cantidadDeColliders / 2)
            _semaforosSalteados++;

        _enSemaforo = false;
        _cantidadDeCollidersPasandoSemaforo = 0;
    }
    
    public void PresentarEvaluacion()
    {
        string resultadoEvaluacion = "El Evaluado paso " + _semaforosSalteados + " semaforos en rojo.";
        Debug.Log(resultadoEvaluacion);
    }

    public void ComenzarEvaluacion()
    {
        _cantidadDeColliders = GetComponentsInChildren<Collider>().Length;
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
