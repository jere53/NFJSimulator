using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Logger : MonoBehaviour
{
    private static List<string> logs = new List<string>();

    public static void Log(string s)
    {
        logs.Add(s);
    }

    void OnDisable()
    {
        Debug.Log("------LOGS:------");
        foreach (var s in logs)
        {
            Debug.Log(s);
        }
    }
}
