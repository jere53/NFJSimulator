using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EvaluationController : NetworkBehaviour
{
    [SerializeField] private MenuPausa _menuPausa;
    public void TogglePause()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            TogglePauseWarningClientRpc();
        }
    }
    
    [ClientRpc]
    public void TogglePauseWarningClientRpc()
    {
        _menuPausa.ToggleShowEvaluatedPauseWarning();
    }
}
