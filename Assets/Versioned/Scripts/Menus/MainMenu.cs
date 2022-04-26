using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void CiudadGTA(){
        SceneManager.LoadScene(2);
    }
    public void CiudadDemo(){
        SceneManager.LoadScene(3);
    }

    public void Ciudad(){
        SceneManager.LoadScene(1);
    }

    public void Tandil() {
        SceneManager.LoadScene(4);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
