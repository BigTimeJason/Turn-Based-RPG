using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem current;
    public Tooltip tooltip;

    private void Awake()
    {
        current = this;
    }

    public static void Show(string content, string header = "")
    {
        current.tooltip.SetText(content, header);
        current.tooltip.gameObject.SetActive(true);
        current.tooltip.gameObject.GetComponent<CanvasGroup>().alpha = 0;
        current.tooltip.gameObject.GetComponent<CanvasGroup>().DOFade(1, 1);
    }
    public static void Hide()
    {
        current.tooltip.gameObject.SetActive(false);
    }
}
