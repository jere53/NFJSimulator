using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class BotonTab : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GrupoTabs grupoTabs;

    public Image background;

    public UnityEvent onTabSelected;

    public UnityEvent onTabDeselected;

    private void Start()
    {
        background = GetComponent<Image>();
        grupoTabs.Subscribe(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        grupoTabs.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        grupoTabs.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        grupoTabs.OnTabExit(this);
    }

    public void Select()
    {
        onTabSelected?.Invoke();
    }

    public void Deselect()
    {
        onTabDeselected?.Invoke();
    }
}
