using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string content;
    public string header;

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipSystem.Show(content, header);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Hide();
    }
}
