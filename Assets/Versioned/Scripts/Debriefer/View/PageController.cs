using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class PageController : MonoBehaviour
{
    private TextMeshProUGUI _textMeshProUGUI;

    private void Awake()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    public void OnPageUp()
    {
        if (_textMeshProUGUI.pageToDisplay < _textMeshProUGUI.textInfo.pageCount)
        {
            _textMeshProUGUI.pageToDisplay++;
        }
    }

    public void OnPageDown()
    {
        if (_textMeshProUGUI.pageToDisplay > 1)
        {
            _textMeshProUGUI.pageToDisplay--;
        }
    }
}
