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
        colliderParaVehiculos.enabled = false;
        yield return new WaitForSeconds(1f);
        ApagarLuces(amarillo);
        PrenderLuces(verde);
        luzActual = 2;

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

        yield return new WaitForSeconds(1f);
        colliderParaVehiculos.enabled = true;
        ApagarLuces(amarillo);
        PrenderLuces(rojo);
        luzActual = 0;

        foreach (var e in zonasEspera)
        {
            e.puedePasar = true;
        }
    }

    public void RojoInmediato()
    {
        if (luzActual == 0) return;
        
        ApagarLuces(verde);
        ApagarLuces(amarillo);
        PrenderLuces(rojo);
        colliderParaVehiculos.enabled = true;
        luzActual = 0;
        
        foreach (var e in zonasEspera)
        {
            e.puedePasar = true;
        }
    }
    
    public void VerdeInmediato()
    {
        if (luzActual == 2) return;
        
        ApagarLuces(rojo);
        ApagarLuces(amarillo);
        PrenderLuces(verde);
        colliderParaVehiculos.enabled = false;
        luzActual = 2;
        
        foreach (var e in zonasEspera)
        {
            e.puedePasar = false;
        }
    }
    
    public void AmarilloInmediato()
    {
        if(luzActual == 1) return;
        ApagarLuces(rojo);
        ApagarLuces(verde);
        PrenderLuces(amarillo);
        colliderParaVehiculos.enabled = false;
        luzActual = 1;
    }
    
}
