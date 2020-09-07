using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverableTextButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Text Text;
    public Color HoverColor = Color.black;
    private Color _defaultColor;

    void Start()
    {
        if (Text == null)
            Text = GetComponentInChildren<Text>();
        _defaultColor = Text.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Text.color = HoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Text.color = _defaultColor;
    }
}
