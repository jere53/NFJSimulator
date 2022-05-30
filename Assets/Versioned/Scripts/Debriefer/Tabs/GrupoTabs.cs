using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GrupoTabs : MonoBehaviour
{
    public List<BotonTab> botonesTab;
    
    public Color tabIdle = Color.gray;
    public Color tabHover = Color.Lerp(Color.green, Color.clear, 0.8f);
    public Color tabActive = Color.green;

    private BotonTab _tabSeleccionada;

    public List<GameObject> objetosACambiar;

    public DebrieferPresenter DebrieferPresenter;
    public void Subscribe(BotonTab botonTab)
    {
        if (botonesTab == null)
        {
            botonesTab = new List<BotonTab>();
        }
        botonesTab.Add(botonTab);
    }

    public void OnTabEnter(BotonTab b) 
    {
        ResetTabs();
        if(_tabSeleccionada == null || b!=_tabSeleccionada)
            b.background.color = tabHover;
    }
    
    public void OnTabExit(BotonTab b)
    {
        ResetTabs();
    }
    
    public void OnTabSelected(BotonTab b)
    {
        if (_tabSeleccionada != null)
        {
            _tabSeleccionada.Deselect();
        }
        
        _tabSeleccionada = b;
        
        _tabSeleccionada.Select();
        
        ResetTabs();
        
        b.background.color = tabActive;
        
        int indice = b.transform.GetSiblingIndex();
        for (int i = 0; i < objetosACambiar.Count; i++)
        {
            if (indice == i)
            {
                objetosACambiar[i].SetActive(true);
            }
            else
            {
                objetosACambiar[i].SetActive(false);
            }
        }
    }

    public void ResetTabs()
    {
        foreach (var boton in botonesTab)
        {
            if(_tabSeleccionada!=null && boton == _tabSeleccionada) continue;
            boton.background.color = tabIdle;
        }
    }

    private void Start()
    {
        foreach (var o in objetosACambiar)
        {
            o.SetActive(false);
        }
    }
}
