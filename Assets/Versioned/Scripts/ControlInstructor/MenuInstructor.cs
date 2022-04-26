using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInstructor : MonoBehaviour
{
    public GameObject condiciones;
    public GameObject ejercicios;
    public GameObject princial;

    public void MenuPrincipal()
    {
        condiciones.SetActive(false);
        ejercicios.SetActive(false);
        princial.SetActive(true);
    }
    
    public void Condiciones()
    {
        princial.SetActive(false);
        condiciones.SetActive(true);
    }

    public void Ejercicios()
    {
        princial.SetActive(false);
        ejercicios.SetActive(true);
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}
