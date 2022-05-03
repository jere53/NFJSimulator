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
            semaforo.RojoInmediato();
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
}
