using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EvaluatorController : NetworkBehaviour
{
    [SerializeField] private Button toggleRainBtn;
    [SerializeField] private WeatherController _weatherController;
    
    private void Awake()
    {
        toggleRainBtn.onClick.AddListener(() =>
        {
            ToggleRainServerRpc();
            _weatherController.RainPreset();
        });
    }

    [ServerRpc(RequireOwnership = false)]
    public void ToggleRainServerRpc()
    {
        _weatherController.RainPreset();
    }
}
