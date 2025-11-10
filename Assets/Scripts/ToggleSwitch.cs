using UnityEngine;
using UnityEngine.UI;

public class ToggleSwitch : MonoBehaviour
{
    [SerializeField] private RectTransform _handle;
    [SerializeField] private Toggle _toggle;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Color _backgroundOnColor, _backgroundOffColor;
    
    public void ToggleChanged()
    {
        _handle.anchoredPosition *= -1.0f;

        if(_toggle.isOn) _backgroundImage.color = _backgroundOnColor;
        else _backgroundImage.color = _backgroundOffColor;
    }
}
