using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorAccidentes : MonoBehaviour
{
    public GameObject menuPausa;
    private bool isPaused;
    // Start is called before the first frame update
    void Start()
    {
        menuPausa.SetActive(false);
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp("y"))
        {
            if (isPaused)
            {
                isPaused = false;
                menuPausa.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                isPaused = true;
                menuPausa.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }
}
