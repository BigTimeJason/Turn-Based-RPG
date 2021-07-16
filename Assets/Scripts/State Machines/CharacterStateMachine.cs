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
    public Slider atb;
    public TextMeshProUGUI healthUI;
    public TextMeshProUGUI nameUI;
    public GameObject selector;

    [Header("STATS")]
    public Character character;
    public TurnState currentState;
    public List<GameObject> target = new List<GameObject>();

    [Header("PRIVATE")]
    private BattleStateMachine battleStateMachine;
    private bool actionStarted;
    private Vector3 startPosition;
    private float atbProgress;
    private readonly float animSpeed = 5f;
    private bool isAlive = true;

    void Start()
    {
        selector.SetActive(false);
        currentState = TurnState.PROCESSING;
        battleStateMachine = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        atbProgress = Random.Range(0, character.baseSpeed);
        startPosition = transform.position;

        character.availableAttacks.Add(character.weapon.weaponAttack);
    }

    void Update()
    {
        switch (currentState)
        {
            case TurnState.PROCESSING:
                if (battleStateMachine.battleState == BattleStateMachine.BattleState.WAIT) UpdateATB();
                break;
            case TurnState.ADDTOLIST:
                battleStateMachine.readyHeroes.Add(this.gameObject);
                currentState = TurnState.WAITING;
                break;
            case TurnState.WAITING:
                break;
            case TurnState.SELECTING:
                break;
            case TurnState.ACTION:
                StartCoroutine(TimeForAction());
                break;
            case TurnState.DEAD:
                if (!isAlive)
                {
                    return;
                }
                else
                {
                    isAlive = false;
                    gameObject.tag = "DeadHero";

                    battleStateMachine.heroes.Remove(gameObject);
                    battleStateMachine.readyHeroes.Remove(gameObject);
                    selector.SetActive(false);
                    battleStateMachine.attackPanel.SetActive(false);
                    battleStateMachine.targetPanel.SetActive(false);
                    battleStateMachine.turnList.RemoveAll(turn => turn.attacker == gameObject.name);
                    battleStateMachine.heroInput = BattleStateMachine.HeroInputState.ACTIVATE;
                }
                break;
            default:
                break;
        }
    }

    void UpdateATB()
    {
        atbProgress += Time.deltaTime * character.currSpeed;
        atb.value = atbProgress;

        if (atbProgress >= 1)
        {
            currentState = TurnState.ADDTOLIST;
        }
    }

    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        // Animate Character
        //Vector3 targetPosition = new Vector3(target.transform.position.x - 5, target.transform.position.y);
        Vector3 targetPosition = transform.position - new Vector3(-2, 0);

        while (MoveTowards(targetPosition))
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        // Damage


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

    public void TakeDamage(float dmg)
    {
        character.currHP -= dmg;
        healthUI.text = "HP\t" + character.currHP;

        if (character.currHP <= 0)
        {
            character.currHP = 0;
            currentState = TurnState.DEAD;
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
            Debug.Log("You selected an attack with 1 target.");
            for (int i = 0; i < battleStateMachine.enemies.Count; i++)
            {
                if (i >= attack.minRange - 1 && i <= attack.maxRange - 1)
                {
                    Debug.Log("This character was in range of this 1 target attack: " + battleStateMachine.enemies[i].name);
                    eligibleTargets.Add(new List<GameObject> { battleStateMachine.enemies[i] });
                }
            }
        }
        else // if damage is AoE
        {
            int option = 0;
            Debug.Log("You selected an attack that hits multiple targets.");
            for (int i = 0; i <= battleStateMachine.enemies.Count - attack.damage.Length; i++)
            {
                if (i >= attack.minRange - 1 && i <= attack.maxRange - 1)
                {
                    eligibleTargets.Add(new List<GameObject>());
                    for (int j = 0; j < attack.damage.Length; j++)
                    {
                        if (attack.damage[j] != 0)
                        {
                            Debug.Log("This character was in range of this AoE attack: " + battleStateMachine.enemies[j + i].name);
                            eligibleTargets[option].Add(battleStateMachine.enemies[j + i]);
                        }
                        else
                        {
                            Debug.Log("Character starting at position " + j + " (" + battleStateMachine.enemies[j + i].name + ") was in range, but the attack didn't have a damage value at this point.");
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

    public void updateUI()
    {
        healthUI.text = "HP\t" + character.currHP;
        nameUI.text = character.name;
    }
}
