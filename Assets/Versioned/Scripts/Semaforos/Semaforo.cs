using System.Collections;
using UnityEngine;

public class Semaforo : MonoBehaviour
{

    [SerializeField] private Collider colliderParaVehiculos;

    [SerializeField] private ZonaEspera[] zonasEspera;

    [SerializeField] private Light[] rojo;
    [SerializeField] private Light[] amarillo;
    [SerializeField] private Light[] verde;

    public int luzActual; //0 rojo, 1 amarillo, 2 verde


    void ApagarLuces(Light[] luces)
    {
        foreach (var l in luces)
        {
            l.enabled = false;
        }
    }

    void PrenderLuces(Light[] luces)
    {
        foreach (var l in luces)
        {
            l.enabled = true;
        }
    }
    
    public IEnumerator Verde()
    {
        ApagarLuces(rojo);
        PrenderLuces(amarillo);
        luzActual = 1;
        yield return new WaitForSeconds(1f);
        ApagarLuces(amarillo);
        PrenderLuces(verde);
        luzActual = 2;

        colliderParaVehiculos.enabled = false;
        
        foreach (var e in zonasEspera)
        {
            e.puedePasar = false;
        }
        
    }

    public IEnumerator Rojo()
    {
        ApagarLuces(verde);
        PrenderLuces(amarillo);
        luzActual = 1;
        colliderParaVehiculos.enabled = true;
        
        foreach (var e in zonasEspera)
        {
            e.puedePasar = true;
        }
        
        yield return new WaitForSeconds(1f);
        ApagarLuces(amarillo);
        PrenderLuces(rojo);
        luzActual = 0;
        colliderParaVehiculos.enabled = true;
        
        foreach (var e in zonasEspera)
        {
            e.puedePasar = true;
        }
    }

    public void RojoInmediato()
    {
        ApagarLuces(verde);
        ApagarLuces(amarillo);
        PrenderLuces(rojo);
        luzActual = 0;
    }
    
    public void VerdeInmediato()
    {
        ApagarLuces(rojo);
        ApagarLuces(amarillo);
        PrenderLuces(verde);
        luzActual = 2;
    }
    
    public void AmarilloInmediato()
    {
        ApagarLuces(rojo);
        ApagarLuces(verde);
        PrenderLuces(amarillo);
        luzActual = 1;
    }
    
}
