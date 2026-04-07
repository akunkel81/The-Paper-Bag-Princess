using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;

    [TextArea(2, 4)]
    public string message;

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipPanel.SetActive(true);
        tooltipText.text = message;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipPanel.SetActive(false);
    }
}