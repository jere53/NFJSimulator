using System.Collections;
using UnityEngine;

public class Peaton : MonoBehaviour
{
    //definir evento OnMuerte
    private SpawnManager _spawnManager;

    private Collider _collider;
    
    private IAPeaton _iaPeaton;
    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _iaPeaton = GetComponent<IAPeaton>();
        _collider = GetComponent<Collider>();
    }

    public void Spawn(Vector3 origen, float distancia, SpawnManager spawner)
    {
        gameObject.SetActive(true);
        _spawnManager = spawner;
        _iaPeaton.Init(origen, distancia);
        _collider.enabled = true;
        _spawnManager.peatonesHabilitados.Add(this);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        //Logger.Log("atropellaron a un peaton " + Time.realtimeSinceStartup + " segundos despues de inicializar la simulacion");
        /*
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Trainee atropello un peaton " + Time.realtimeSinceStartup + " segundos despues de inicializar la simulacion");
        }
        */

        Rigidbody rb = other.attachedRigidbody;
        Vector3 rbVel = rb.velocity;
        Vector3 fuerzaDeImpacto = rbVel * 0.5f * rbVel.sqrMagnitude * rb.mass;
        
        
        if (fuerzaDeImpacto.magnitude > 10000f)
        {
            StartCoroutine(Morir(10f, fuerzaDeImpacto));
        }
    }

    public IEnumerator Morir(float tiempoHastaRespawn, Vector3 fuerzaRecibida)
    {
        _iaPeaton.Detener();
        _iaPeaton.enabled = false;
        _collider.enabled = false;
        _animator.SetTrigger("atropellado");
        yield return new WaitForSeconds(tiempoHastaRespawn);
        _spawnManager.ReSpawnPeaton(this);
    }
    
    
    public void Animar(float velociodad)
    {
        _animator.SetFloat("velocidad", velociodad);
    }
}
