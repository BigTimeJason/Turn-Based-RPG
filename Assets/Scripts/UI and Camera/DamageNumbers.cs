using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class DamageNumbers : MonoBehaviour
{
    private static DamageNumbers _instance;
    public static DamageNumbers Instance { get { return _instance; } }

    public GameObject damageNumberPrefab;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void Show(float damage, GameObject target)
    {
        GameObject curr = Instantiate(damageNumberPrefab, this.transform);
        curr.transform.position = target.transform.position;
        if(damage > 0)
        {
            curr.GetComponent<TextMeshProUGUI>().color = new Color(1,1,1);
        } else
        {
            curr.GetComponent<TextMeshProUGUI>().color = new Color(0,1,0.1f);
        }
        curr.GetComponent<TextMeshProUGUI>().SetText("" + Mathf.Abs(damage));
        curr.transform.DOMoveY(curr.transform.position.y + 2, 3).OnComplete(() => { Destroy(curr); });
        curr.GetComponent<TextMeshProUGUI>().DOColor(new Color(0, 0, 0, 0), 1);
    }
}
