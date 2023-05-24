using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerController : MonoBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button rainBtn;

    [SerializeField] private WeatherController _weatherController;

    private void Awake()
    {
        serverBtn.onClick.AddListener(() => NetworkManager.Singleton.StartServer());
        clientBtn.onClick.AddListener(() => NetworkManager.Singleton.StartClient());
        hostBtn.onClick.AddListener(() => NetworkManager.Singleton.StartHost());
        rainBtn.onClick.AddListener(() => _weatherController.RainPreset());
    }  
}
