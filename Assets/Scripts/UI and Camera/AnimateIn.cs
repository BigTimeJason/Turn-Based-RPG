using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class AnimateIn : MonoBehaviour
{
    private Vector3 startPos;
    public float delay;
    public Image buttonImage;

    private void Start()
    {
        startPos = transform.position;
        GetComponent<Image>().color = new Color(1, 1, 1, 0);
        buttonImage.color = new Color(1, 1, 1, 0); 
        transform.position -= new Vector3(50, 0);

        StartCoroutine(AnimateCoroutine(delay));
    }

    IEnumerator AnimateCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        transform.DOMoveX(startPos.x, 1f);
        GetComponent<Image>().DOColor(new Color(1, 1, 1, 1), 1);
        buttonImage.DOColor(new Color(1, 1, 1, 1), 1);
    }
}
