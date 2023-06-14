using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handle UI Menu
/// </summary>
public class UIMananger : MonoBehaviour
{
    // Buttons
    [SerializeField] private Button evaluatedBtn;
    [SerializeField] private Button evaluatorBtn;
    
    // Others
    [SerializeField] private SpawnManager _spawnManager; // TODO : Move this to another class
    [SerializeField] private MenuPausa _menuPausaManager;
    
    // Menus
    [SerializeField] private GameObject ModeSelectionMenu;
    
    private String MODE_SELECTOR_MENU = "ModeSelectionMenu";
    private String CURRENT_MENU;
    
    private Dictionary<String, GameObject> _menus = new Dictionary<string, GameObject>();
    
    private void Start()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();
        _menus.Add(MODE_SELECTOR_MENU, ModeSelectionMenu);
        ActivateMenu(MODE_SELECTOR_MENU);
    }

    /// <summary>
    /// Activate menu by name
    /// </summary>
    private void ActivateMenu(String menu)
    {
        foreach (var m in _menus)
        {
            if (m.Key == menu)
            {
                m.Value.SetActive(true);
            }
            else
            {
                m.Value.SetActive(false);
            }
        }
    }
    
    private void DesactivarMenus()
    {
        foreach (var m in _menus)
        {
            m.Value.SetActive(false);
        }
    }

    private void Awake()
    {
        
        evaluatedBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            _spawnManager.HandleSpawnVehiculosYPeatones();
            DesactivarMenus();
            _menuPausaManager.SetCurrentPauseMenu(MenuPausa.PauseMenuEnum.MenuEvaluado);
        });
        
        evaluatorBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            DesactivarMenus();
            _menuPausaManager.SetCurrentPauseMenu(MenuPausa.PauseMenuEnum.MenuEvaluador);
        });
        
    }
    
}
