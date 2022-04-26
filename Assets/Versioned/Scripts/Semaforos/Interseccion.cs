using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interseccion : MonoBehaviour
{
    public List<Semaforo> semaforos;
    
    public int tiempoCadaSemaforoEnVerde;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var semaforo in semaforos)
        {
            semaforo.StartCoroutine(semaforo.Rojo());
        }

        StartCoroutine(InicInterseccion());

        /*
        if (TryGetComponent<CapturadorInterseccion>(out var cap))
        {
            cap.ComenzarARegistrar();
        }
        */
    }

    IEnumerator InicInterseccion()
    {
        while (true)
        {
            yield return null; //por si no hay semaforos.
            int cantSemaforos = semaforos.Count;
            for (int i = 0; i < cantSemaforos; i++)
            {
                semaforos[i].StartCoroutine(semaforos[i].Rojo());
                semaforos[(i + 1) % cantSemaforos].StartCoroutine(semaforos[(i + 1) % cantSemaforos].Verde());
                yield return new WaitForSeconds(tiempoCadaSemaforoEnVerde);
            }
        }
    }

    public IEnumerator InicInterseccionDelayed(float delay)
    {
        Debug.Log("Hmmmm " + delay);
        int cantSemaforos = semaforos.Count;
        Debug.Log("cant "+ cantSemaforos);
        int semaforoActual = (int) Mathf.Floor(delay / tiempoCadaSemaforoEnVerde) % cantSemaforos + 1;
        //+1 porque se empieza con el semaforo en rojo y el siguiente se pone en verde, 
        //ya que en el for de la linea 32 se pone "semaforos[(i + 1) % cantSemaforos]" en verde al inicio
        Debug.Log("semAct " + semaforoActual);
        float tiempoEnVerdeSemaforActual = (tiempoCadaSemaforoEnVerde-1) - (delay % (tiempoCadaSemaforoEnVerde-1));
        //-1 porque como la corrutina Verde() espera un segundo antes de poner la luz verde, el semaforo en realidad
        //pasa tiempoCadaSemaforoEnVerde-1 segundos en verde.

        for (int i = 0; i < cantSemaforos; i++)
        {
            semaforos[i].RojoInmediato();
        }

        semaforos[semaforoActual].VerdeInmediato(); 
        yield return new WaitForSeconds(tiempoEnVerdeSemaforActual);
        
        while (true)
        {
            yield return null;
            for (int i = semaforoActual; i < cantSemaforos; i++)
            {
                semaforos[i].StartCoroutine(semaforos[i].Rojo());
                semaforos[(i + 1) % cantSemaforos].StartCoroutine(semaforos[(i + 1) % cantSemaforos].Verde());
                yield return new WaitForSeconds(tiempoCadaSemaforoEnVerde);
            }
        }
    }
}
