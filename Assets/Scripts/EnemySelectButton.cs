using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySelectButton : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;

    public void SelectEnemy()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().Input2(enemyPrefabs);
    }

    public void ToggleSelector()
    {
        foreach (GameObject enemyPrefab in enemyPrefabs)
        {
            enemyPrefab.transform.Find("Selector").gameObject.SetActive(!enemyPrefab.transform.Find("Selector").gameObject.activeSelf);
        }
    }
}
