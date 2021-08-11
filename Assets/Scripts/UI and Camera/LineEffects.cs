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

    public void Draw(GameObject source, GameObject target)
    {
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, source.transform.position);
        lineRenderer.SetPosition(1, target.transform.position + new Vector3(Random.Range(-variance, variance), Random.Range(-variance, variance)));
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
