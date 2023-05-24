using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private GameObject MenuEvaluator;
    [SerializeField] private GameObject NetworkMenu;
    private void Start()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();
    }

    private void Awake()
    {
        serverBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        });
        
        hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            _spawnManager.HandleSpawnVehiculosYPeatones();
            
        });
        
        clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            NetworkMenu.SetActive(false);
            MenuEvaluator.SetActive(true);
        });
    }
}
