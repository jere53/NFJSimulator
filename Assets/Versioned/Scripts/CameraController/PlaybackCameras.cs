using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaybackCameras : MonoBehaviour
{
    private GameObject[] CameraList;
    private string tag = "CameraPlayback";
    private int nextCamera = 0;
    
    // Start is called before the first frame update
    void Start()
    {   

    }

    // Update is called once per frame
    void Update()
    {
        CameraList = GameObject.FindGameObjectsWithTag(tag);
        
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log(CameraList[nextCamera]);
            Camera.main.transform.SetParent(CameraList[nextCamera].transform);
            Camera.main.transform.localPosition = new Vector3(0, 0, 0);
            Camera.main.transform.localRotation =  Quaternion.Euler(0, 0, 0);
            nextCamera++;
        }
        if (nextCamera == CameraList.Length)
        {
            nextCamera = 0;
        }
    }


   
}
