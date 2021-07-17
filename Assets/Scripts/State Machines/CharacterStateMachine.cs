using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterStateMachine : MonoBehaviour
{
    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        SELECTING,
        ACTION,
        DEAD
    }

    [Header("UI")]
    public GameObject selector;

    [Header("STATS")]
    public Character character;
    public TurnState currentState;
    public List<GameObject> targets = new List<GameObject>(); 
    public BattleStateMachine battleStateMachine;
    public Vector3 startPosition;
    public float atbProgress;
    public readonly float animSpeed = 5f;
    public bool isAlive = true;
    public bool actionStarted = false;

    void Start()
    {
        selector.SetActive(false);
        currentState = TurnState.PROCESSING;
        battleStateMachine = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        atbProgress = Random.Range(0, character.baseSpeed);
        startPosition = transform.position;

        Action meleeAttack = Action.CreateInstance<Action>();
        meleeAttack.Init("Melee", "A melee attack", new float[] { 1.2f }, 1, 1);
        character.availableActions.Add(new CharacterAction(new List<Action>() {meleeAttack}));
    }

    public virtual void UpdateATB()
    {
        atbProgress += Time.deltaTime * character.currSpeed;

        if (atbProgress >= 1)
        {
            currentState = TurnState.ADDTOLIST;
        }
    }

    public IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        // Animate Character
        Vector3 targetPosition = transform.position + new Vector3(0, 1);

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

    private bool MoveTowards(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    public virtual void TakeDamage(float dmg)
    {
        character.currHP -= dmg;

        if (character.currHP <= 0)
        {
            character.currHP = 0;
            currentState = TurnState.DEAD;
        }
    }
    private void DealDamage()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            float dmg = battleStateMachine.turnList[0].attack.damage[i];
            targets[i].GetComponent<CharacterStateMachine>().TakeDamage(dmg);
        }
    }

    public List<GameObject> PickTargetFromEligibleTargets(List<List<GameObject>> eligibleTargets)
    {
        return eligibleTargets[Random.Range(0, eligibleTargets.Count)];
    }

    public List<List<GameObject>> GetEligibleTargets(Action attack)
    {
        List<List<GameObject>> eligibleTargets = new List<List<GameObject>>();
        List<GameObject> team;
        if (gameObject.CompareTag("Hero"))
        {
            team = battleStateMachine.enemies;
        } else
        {
            team = battleStateMachine.heroes;
        }
        if (attack.damage.Length == 1)
        {
            Debug.Log("You selected an attack with 1 target.");
            for (int i = 0; i < team.Count; i++)
            {
                if (i >= attack.minRange - 1 && i <= attack.maxRange - 1)
                {
                    Debug.Log("This character was in range of this 1 target attack: " + team[i].name);
                    eligibleTargets.Add(new List<GameObject> { team[i] });
                }
            }
        }
        else // if damage is AoE
        {
            int option = 0;
            Debug.Log("You selected an attack that hits multiple targets.");
            for (int i = 0; i <= team.Count - attack.damage.Length; i++)
            {
                if (i >= attack.minRange - 1 && i <= attack.maxRange - 1)
                {
                    eligibleTargets.Add(new List<GameObject>());
                    for (int j = 0; j < attack.damage.Length; j++)
                    {
                        if (attack.damage[j] != 0)
                        {
                            Debug.Log("This character was in range of this AoE attack: " + team[j + i].name);
                            eligibleTargets[option].Add(team[j + i]);
                        }
                        else
                        {
                            Debug.Log("Character starting at position " + j + " (" + team[j + i].name + ") was in range, but the attack didn't have a damage value at this point.");
                        }
                    }
                    option++;
                }
                else
                {
                    Debug.Log("Character starting at position " + i + " was not in range.");
                }
            }
        }

        Debug.Log("This list's length is..." + eligibleTargets.Count);
        return eligibleTargets;
    }

    public bool HasEligibleTargets(List<List<GameObject>> eligibleTargets)
    {
        return eligibleTargets.Count != 0;
    }
}
