using System;
using UnityEngine;
using System.Collections;

public class Sirena : MonoBehaviour {

    [SerializeField] GameObject luzRoja;
    [SerializeField] GameObject luzAzul;
    [SerializeField] GameObject lucesCuerpo;

    private Vector3 redTemp;
    private Vector3 blueTemp;
    public bool sirenaActiva;
    private Coroutine c1;
    private AudioSource sonido;

    [SerializeField] float speed;

    private void Start()
    {
        luzRoja.SetActive(false);
        luzAzul.SetActive(false);
        lucesCuerpo.SetActive(false);
        sirenaActiva = false;
        sonido = GetComponent<AudioSource>();
        sonido.enabled = false;
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleSirena();
        }
    }

    public void ToggleSirena()
    {
        if (!sirenaActiva)
        {
            sirenaActiva = true;
            c1 = StartCoroutine(ControlarLuz());
            sonido.enabled = true;
        }
        else
        {
            StopCoroutine(c1);
            luzRoja.SetActive(false);
            luzAzul.SetActive(false);
            lucesCuerpo.SetActive(false);
            sirenaActiva = false;
            sonido.enabled = false;
        }
    }

    public void Encender()
    {
        Debug.Log("entro");
        if(sirenaActiva) return;
        Debug.Log("deberia andar gahhsg");
        sirenaActiva = true;
        c1 = StartCoroutine(ControlarLuz());
        sonido.enabled = true;
    }

    public void Apagar()
    {
        if(!sirenaActiva) return;
        StopCoroutine(c1);
        luzRoja.SetActive(false);
        luzAzul.SetActive(false);
        lucesCuerpo.SetActive(false);
        sirenaActiva = false;
        sonido.enabled = false;
    }


    IEnumerator ControlarLuz()
    {
        while (true)
        {
            luzRoja.SetActive(true); // la performance de este metodo no tiene que ser un problema, a no ser que se llame cientos o miles de veces por frame
            lucesCuerpo.SetActive(true);
            yield return new WaitForSecondsRealtime(speed);
            luzRoja.SetActive(false);  
            lucesCuerpo.SetActive(false);
            yield return new WaitForSecondsRealtime(speed);
            luzRoja.SetActive(true);
            lucesCuerpo.SetActive(true);
            yield return new WaitForSecondsRealtime(speed);
            luzRoja.SetActive(false);  
            lucesCuerpo.SetActive(false);
            yield return new WaitForSecondsRealtime(speed);
            luzAzul.SetActive(true);
            lucesCuerpo.SetActive(true);
            yield return new WaitForSecondsRealtime(speed);
            luzAzul.SetActive(false);  
            lucesCuerpo.SetActive(false);
            yield return new WaitForSecondsRealtime(speed);
            luzAzul.SetActive(true);
            lucesCuerpo.SetActive(true);
            yield return new WaitForSecondsRealtime(speed);
            luzAzul.SetActive(false);  
            lucesCuerpo.SetActive(false);
            yield return new WaitForSecondsRealtime(speed);
            
            
        }
        
    }
}