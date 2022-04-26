using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriterioEvitarAccidentes : MonoBehaviour, ICriterio
{
    private bool _evaluando;

    private int _peatonesGolpeados;
    private int _colisionesContraVehiculos;
    
    private Dictionary<int, bool> _idsVehiculosGolpeados = new Dictionary<int, bool>(); //para que no reporte mas de una colision contra un mismo vehiculo.

    private void OnTriggerEnter(Collider other)
    {
        if (!_evaluando) return;
        if (other.CompareTag("Peaton"))
            _peatonesGolpeados++;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!_evaluando) return;
        if (other.gameObject.CompareTag("Auto"))
        {
            _colisionesContraVehiculos++;
            if (!_idsVehiculosGolpeados.ContainsKey(other.gameObject.GetInstanceID())){
                _idsVehiculosGolpeados.Add(other.gameObject.GetInstanceID(), true);
            }

        }
    }

    public void PresentarEvaluacion()
    {
        string resultadoEvaluacion = "El Evaluado golpeo a " + _peatonesGolpeados + " peatones, y tuvo " +
                                     _colisionesContraVehiculos + " colisiones con " +
                                     _idsVehiculosGolpeados.Count + " vehiculos diferentes.";
        Debug.Log(resultadoEvaluacion);
    }

    public void ComenzarEvaluacion()
    {
        _peatonesGolpeados = 0;
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
