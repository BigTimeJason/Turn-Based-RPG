using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LineEffects : MonoBehaviour
{
    private static LineEffects _instance;
    public static LineEffects Instance { get { return _instance; } }

    public LineRenderer lineRenderer;
    [Range(0, 1)]
    public float variance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void Fire(Vector3 source, List<GameObject> gameObjects, Element element)
    {
        //lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;

        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(GetColours.GetColourOfElement(element), 0), new GradientColorKey(GetColours.GetColourOfElement(element), 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;

        //lineRenderer.SetColors(GetColours.GetColourOfElement(element), GetColours.GetColourOfElement(element));

        //Debug.Log(GetColours.GetColourOfElement(element));
        lineRenderer.SetPosition(0, source);
        lineRenderer.SetPosition(1, gameObjects[Random.Range(0, gameObjects.Count)].transform.position + new Vector3(Random.Range(-variance, variance), Random.Range(-variance, variance)));
    }

    private void Update()
    {
        if (lineRenderer.startWidth >= 0)
        {
            lineRenderer.startWidth -= 0.5f * Time.deltaTime;
            lineRenderer.endWidth -= 0.5f * Time.deltaTime;
        } else
        {
            lineRenderer.positionCount = 0;
        }
    }
}
