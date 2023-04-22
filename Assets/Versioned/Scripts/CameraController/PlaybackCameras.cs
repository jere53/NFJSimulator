using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaybackCameras : MonoBehaviour
{
    private GameObject[] CameraList;
    private string tag = "CameraPlayback";
    private int nextCamera = 0;
    

    // Update is called once per frame
    void Update()
    {
        CameraList = GameObject.FindGameObjectsWithTag(tag); // TODO : Review
    }

    public void ChangeCamera()
    {
        Camera.main.transform.SetParent(CameraList[nextCamera].transform);
        Camera.main.transform.localPosition = new Vector3(0, 0, 0);
        Camera.main.transform.localRotation =  Quaternion.Euler(0, 0, 0);
        nextCamera++;
        if (nextCamera == CameraList.Length)
        {
            nextCamera = 0;
        }
    }
}
