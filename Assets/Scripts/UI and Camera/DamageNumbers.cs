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

    public void Show(float damage, List<GameObject> targets)
    {
        foreach (GameObject target in targets) {
            GameObject curr = Instantiate(damageNumberPrefab, this.transform);
            curr.transform.position = target.transform.position;
            curr.GetComponent<TextMeshProUGUI>().SetText(""+damage);
            curr.transform.DOMoveY(curr.transform.position.y + 2, 3).OnComplete(() => { Destroy(curr); });
            curr.GetComponent<TextMeshProUGUI>().DOColor(new Color(0, 0, 0, 0), 1);
        }
        
    }
}
