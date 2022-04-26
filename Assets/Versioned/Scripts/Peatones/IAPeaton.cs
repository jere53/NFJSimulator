using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IAPeaton : MonoBehaviour
{
    private NavMeshAgent peatonAgent;

    private Peaton _peaton;

    public float distanciaBuscaCamino = 100f; //para poder probar diferentes valores a ver cual anda mejor 

    public void Init(Vector3 origen, float distancia) //inicializa el peaton cerca de una posicion
    {
        enabled = true; //que el monobehaviour empieze a ser Updateado de nuevo.
        _estaEsperando = false;
        _zonaEsperaActual = null;
        //movemos el peaton a una posicion sobre la vereda en un rango de 100 metros alrededor del origen
        transform.position = ConseguirPosicionAleatoria(origen, distancia, 1 << 4);
        peatonAgent.enabled = true;
        peatonAgent.Warp(transform.position); //workaround para un error de NMA que hace que no se sincronize la posicion al instanciarse.
        NavMeshPath path = new NavMeshPath();
        if (peatonAgent.CalculatePath(ConseguirPosicionAleatoria(transform.position, distanciaBuscaCamino, 1 << 4), path))
        {
            peatonAgent.path = path;
        } //que empieze a caminar.
        else Debug.Log("Path Invalido");

        StartCoroutine(TratarAgenteAtrapado());
    }

    public void Detener()
    {
        if (!peatonAgent.enabled) return; //por si lo queremos detener desde mas de un lugar. Que solo se detenga una vez.

        _estaEsperando = false;
        _zonaEsperaActual = null;
        peatonAgent.isStopped = true;
        peatonAgent.enabled = false;
        StopCoroutine(TratarAgenteAtrapado());
    }

    //Devuelve una posicion aleatoria en la capa especificada de la NavMesh
    public Vector3 ConseguirPosicionAleatoria(Vector3 origen, float radio, int layermask)
    {
        Vector3 direccionAleatoria = UnityEngine.Random.insideUnitSphere * radio;
        direccionAleatoria += origen;
        NavMeshHit navMeshHit;
        NavMesh.SamplePosition(direccionAleatoria, out navMeshHit, radio, layermask);
        return navMeshHit.position;
    }

    private void Awake()
    {
        peatonAgent = GetComponent<NavMeshAgent>();
        _peaton = GetComponent<Peaton>();
    }
    

    // Update is called once per frame
    void Update()
    {
        if (!peatonAgent.enabled) return;

        CheckZonaEspera();

        peatonAgent.isStopped = _estaEsperando;
        
        //si llego a destino, que se siga moviendo.
        if (!_estaEsperando && peatonAgent.isStopped)
        {
            peatonAgent.SetDestination(ConseguirPosicionAleatoria(transform.position, distanciaBuscaCamino, 1 << 4));
        }
    }
    
    //----------------------------------------------------------------------------
    //Interaccion Con Semaforos
    private bool _estaEsperando;
    
    private ZonaEspera _zonaEsperaActual;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ZonaEspera"))
        {
            ZonaEspera zonaEspera = other.GetComponent<ZonaEspera>();
            if (EsZonaEsperaOpuesta(zonaEspera))
                return; //significa que se venimos desde la calle, no hay que parar en ese caso
            _zonaEsperaActual = zonaEspera;
            if (!zonaEspera.puedePasar)
                _estaEsperando = true;
        }
    }

    bool EsZonaEsperaOpuesta(ZonaEspera zonaEspera)
    {
        if (zonaEspera.zonaEsperaOpuesta)
        {
            if (zonaEspera.zonaEsperaOpuesta == _zonaEsperaActual)
                return true;
        }

        return false;
    }

    void CheckZonaEspera()
    {
        if (_estaEsperando)
        {
            if (_zonaEsperaActual)
                _estaEsperando = !_zonaEsperaActual.puedePasar;
        }
    }
        //----------------------------------------------------------------------------
    


    IEnumerator TratarAgenteAtrapado()
    {
        while (true)
        {
            if (!peatonAgent.enabled) yield break;
            
            if (!_estaEsperando && peatonAgent.velocity.magnitude < 0.2f)
            {
                NavMeshPath path = new NavMeshPath();
                if (peatonAgent.CalculatePath(
                    ConseguirPosicionAleatoria(transform.position, distanciaBuscaCamino, 1 << 4), path))
                {
                    peatonAgent.path = path;
                } //que empieze a caminar.
            }

            yield return new WaitForSeconds(2f);
        }

        
    }

    private void LateUpdate()
    {
        if (!peatonAgent.enabled)
        {
            _peaton.Animar(0);
            return;
        }
        
        _peaton.Animar(peatonAgent.velocity.magnitude);
    }
}
