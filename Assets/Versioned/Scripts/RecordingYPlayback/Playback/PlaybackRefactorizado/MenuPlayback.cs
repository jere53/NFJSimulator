using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
public class MenuPlayback : MonoBehaviour
{
    public GameObject simulacion;
    public InputField inputField;
    public ControladorGrabacion controladorGrabacion;
    public Text mensajeError;
    
    // Start is called before the first frame update
    void Start()
    {
        simulacion.SetActive(false);
    }

    public void AbrirArchivo()
    {
        mensajeError.text = "";
        inputField.text = EditorUtility.OpenFolderPanel("Cargar Grabacion", "","");
    }

    public void Reproducir()
    {
        string path = inputField.text;
        
        string[] files = Directory.GetFiles(path);
        string pathGrabacion = "";
        string pathClima = "";
        string pathHeader = "";
        
        foreach (string file in files) //REVISAR. acomodarlo a los nombres de archivos que cambio jere
        {
            if (file.EndsWith("grabacion.nfj"))
                pathGrabacion = file;
            else if (file.EndsWith("grabacionClimaCiclo.nfj"))
                pathClima = file;
            else if (file.EndsWith("grabacionHeader.nfj"))
                pathHeader = file;
        }

        if (pathGrabacion != "" && pathClima != "" && pathHeader != "")
        {
            controladorGrabacion.pathGrabacion = pathGrabacion;
        controladorGrabacion.pathClima = pathClima;
        controladorGrabacion.pathHeader = pathHeader;
        simulacion.SetActive(true);
        controladorGrabacion.ComenzarPlayback(0.3f, true);
        gameObject.SetActive(false);
        }
        else
        {
            mensajeError.text = "¡Ruta incorrecta!";
        }
    }
    
    
}
