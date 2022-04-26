using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MostrarSlider : MonoBehaviour
{
    public TextMeshProUGUI _textMeshPro;
    public Slider slider;

    public void ActualizarValor()
    {
       _textMeshPro.text = "" + slider.value;
    }
}
