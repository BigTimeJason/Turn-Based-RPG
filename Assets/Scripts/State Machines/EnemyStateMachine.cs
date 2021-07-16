using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyStateMachine : MonoBehaviour
{
    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        ACTION,
        DEAD
    }

    [Header("UI")]
    public GameObject selector;

    [Header("STATS")]
    public Character character;
    public TurnState currentState;
    public List<GameObject> targets = new List<GameObject>();

    [Header("PRIVATE")]
    private BattleStateMachine battleStateMachine;
    private Vector3 startPosition;
    private float atbProgress;
    private readonly float animSpeed = 5f;

    private bool actionStarted = false;

    void Start()
    {
        selector.SetActive(false);
        currentState = TurnState.PROCESSING;
        battleStateMachine = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        atbProgress = Random.Range(0, character.baseSpeed);
        startPosition = transform.position;
    }

    void Update()
    {
        switch (currentState)
        {
            case TurnState.PROCESSING:
                if (battleStateMachine.battleState == BattleStateMachine.BattleState.WAIT) UpdateATB();
                break;
            case TurnState.ADDTOLIST:
                ChooseAction();
                currentState = TurnState.WAITING;
                break;
            case TurnState.WAITING:
                break;
            case TurnState.ACTION:
                StartCoroutine(TimeForAction());
                break;
            case TurnState.DEAD:
                break;
            default:
                break;
        }
    }

    void UpdateATB()
    {
        atbProgress += Time.deltaTime * character.currSpeed;

        if(atbProgress >= 1)
        {
            currentState = TurnState.ADDTOLIST;
        }
    }

    void ChooseAction()
    {
        BaseAttack currentAttack = character.availableAttacks[Random.Range(0, character.availableAttacks.Count)];
        if (GetEligibleTargets(currentAttack).Count == 0)
        {
            return;
        }
        HandleTurn attack = new HandleTurn(character.name, "enemy", gameObject, PickTargetFromEligibleTargets(GetEligibleTargets(currentAttack)), character.availableAttacks[Random.Range(0, character.availableAttacks.Count)]);
        battleStateMachine.AddAction(attack);
    }

    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        // Animate Character
        // Vector3 targetPosition = new Vector3(target.transform.position.x + 5, target.transform.position.y);
        Vector3 targetPosition = transform.position - new Vector3(+2, 0);

        while (MoveTowards(targetPosition))
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        // Damage
        DealDamage();

        while (MoveTowards(startPosition))
        {
            yield return null;
        }

        // Remove action from Battle State Machine
        battleStateMachine.turnList.RemoveAt(0);

        // Reset Battle State Machine
        battleStateMachine.battleState = BattleStateMachine.BattleState.WAIT;

        actionStarted = false;
        atbProgress = 0;
        currentState = TurnState.PROCESSING;
    }

    private bool MoveTowards (Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    private void DealDamage()
    {
        for(int i = 0; i < targets.Count; i++)
        {
            float dmg = battleStateMachine.turnList[0].attack.damage[i];
            targets[i].GetComponent<HeroStateMachine>().TakeDamage(dmg);
        }
    }

    public List<GameObject> PickTargetFromEligibleTargets(List<List<GameObject>> eligibleTargets)
    {
        return eligibleTargets[Random.Range(0, eligibleTargets.Count)];
    }

    public List<List<GameObject>> GetEligibleTargets(BaseAttack attack)
    {
        List<List<GameObject>> eligibleTargets = new List<List<GameObject>>();
        if (attack.damage.Length == 1)
        {
            for (int i = 0; i < battleStateMachine.heroes.Count; i++)
            {
                if (i >= attack.minRange - 1 && i <= attack.maxRange - 1)
                {
                    eligibleTargets.Add(new List<GameObject> { battleStateMachine.heroes[i] }) ;
                }
            }
        } else // if damage is AoE
        {
            for (int i = 0; i <= battleStateMachine.heroes.Count - attack.damage.Length; i++)
            {
                eligibleTargets.Add(new List<GameObject>());
                for (int j = i; j < i + attack.damage.Length && j >= attack.minRange - 1; j++)
                {
                    eligibleTargets[i].Add(battleStateMachine.heroes[j]);
                }
            }
        }
        return eligibleTargets;
    }
}
