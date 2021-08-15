using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterStateMachine : MonoBehaviour
{
    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        SELECTING,
        ACTION,
        DEAD,
        WON
    }

    [Header("UI")]
    public GameObject selector;
    public GameObject targeter;
    public Transform gunBarrel;
    public Animator animator;
    public ParticleSystem shieldParticles;

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
        battleStateMachine = BattleStateMachine.Instance;
        gameObject.name = character.name;
        atbProgress = Random.Range(0, character.baseSpeed);
        animator = GetComponent<Animator>();
    }

    public virtual void InitBattle()
    {
        selector.SetActive(false);
        currentState = TurnState.PROCESSING;
        startPosition = transform.position;

        //character.availableActions.Clear();
        //Action meleeAttack = Action.CreateInstance<Action>();
        //meleeAttack.Init(TargetType.ENEMY, character.offenseElement, "Knife", "A melee attack, dealing 100% damage. [1]", new float[] { 1f }, 1, 1);
        //character.availableActions.Add(new CharacterAction(new List<Action>() { meleeAttack }));
        
        //if (character.weapon != null)
        //{
        //    character.weapon.weaponAttack.element = character.weapon.element;
        //    character.AddAction(new CharacterAction(new List<Action>() { character.weapon.weaponAttack }), character.weapon.weaponAttack.actionName);
        //}
    }

    void Update()
    {
        switch (currentState)
        {
            case TurnState.PROCESSING:
                if (battleStateMachine.battleState == BattleStateMachine.BattleState.WAIT) UpdateATB();
                break;
            case TurnState.ADDTOLIST:
                currentState = TurnState.WAITING;
                break;
            case TurnState.WAITING:
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
                    gameObject.tag = "DeadEnemy";

                    battleStateMachine.enemies.Remove(gameObject);
                    selector.SetActive(false);
                    targeter.SetActive(false);
                    battleStateMachine.turnList.RemoveAll(turn => turn.attacker == gameObject.name);
                    battleStateMachine.battleState = BattleStateMachine.BattleState.CHECKALIVE;

                    battleStateMachine.CharacterDied(gameObject);
                }
                break;
            case TurnState.SELECTING:
                break;
            case TurnState.WON:
                break;
            default:
                break;
        }
    }

    public virtual void UpdateATB()
    {
        atbProgress += Time.deltaTime * character.currSpeed;

        if (atbProgress >= 100)
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
        Vector3 targetPosition = transform.position + new Vector3(0, 0);

        //while (MoveTowards(targetPosition))
        //{
        //    yield return null;
        //}

        yield return new WaitForSeconds(2f);

        // Damage
        DealDamage();

        //while (MoveTowards(startPosition))
        //{
        //    yield return null;
        //}

        // Remove action from Battle State Machine
        battleStateMachine.turnList.RemoveAt(0);

        // Reset Battle State Machine
        // battleStateMachine.battleState = BattleStateMachine.BattleState.WAIT;
        
        // We might need to check if the BSM is won or lost here to perform these next 3 lines, but this shouldn't be a problem.
        battleStateMachine.CharacterFinishedAnimation();
        atbProgress = 0;
        currentState = TurnState.PROCESSING;

        actionStarted = false;
        if (animator != null)
        {
            animator.Play("Idle");
        }
    }

    public bool MoveTowards(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    public virtual void TakeDamage(float dmg, Element element)
    {
        if (character.shieldCurrHP > 0)
        {
            if (character.shieldElement == element)
            {
                dmg = dmg * 2;
                character.shieldCurrHP -= dmg;
                if (character.shieldCurrHP <= 0)
                {
                    character.shieldCurrHP = 0;
                    ParticleSystem.MainModule settings = shieldParticles.main;
                    settings.startColor = new ParticleSystem.MinMaxGradient(GetColours.GetColourOfElement(element));
                    shieldParticles.Play();
                }
            } else
            {
                character.shieldCurrHP -= dmg * 0.5f;
                if (character.shieldCurrHP <= 0)
                {
                    character.shieldCurrHP = 0;
                }
            }
        }
        else
        {
            character.currHP -= dmg;
        }

        DamageNumbers.Instance.Show(dmg, this.gameObject);

        if (character.currHP <= 0)
        {
            character.currHP = 0;
            currentState = TurnState.DEAD;
        }
    }

    public void Push(int spaces)
    {

    }
    private void DealDamage()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] != null)
            {
                float dmg = battleStateMachine.turnList[0].attack.damage[i] * (character.currPower / 10) + (Random.Range(-character.currPower/100, character.currPower / 100));
                dmg = (int)dmg;
                targets[i].GetComponent<CharacterStateMachine>().TakeDamage(dmg, battleStateMachine.turnList[0].attack.element);
            }
        }
    }

    public List<GameObject> PickTargetFromEligibleTargets(List<List<GameObject>> eligibleTargets)
    {
        return eligibleTargets[Random.Range(0, eligibleTargets.Count)];
    }

    public List<List<GameObject>> GetEligibleTargets(Action attack)
    {
        List<List<GameObject>> eligibleTargets = new List<List<GameObject>>();
        List<GameObject>[] teams = new List<GameObject>[] { battleStateMachine.heroes, battleStateMachine.enemies };
        int targetedTeamIndex = 0;

        targetedTeamIndex = (gameObject.CompareTag("Hero") ^ attack.targetType == TargetType.FRIENDLY) ? 1 : 0;

        if (attack.damage.Length == 1)
        {
            //Debug.Log("You selected an attack with 1 target.");
            for (int i = 0; i < teams[targetedTeamIndex].Count; i++)
            {
                if (i >= attack.minRange - 1 && i <= attack.maxRange - 1)
                {
                    //Debug.Log("This character was in range of this 1 target attack: " + team[i].name);
                    eligibleTargets.Add(new List<GameObject> { teams[targetedTeamIndex][i] });
                }
            }
        }
        else // if damage is AoE
        {
            int option = 0;
            //Debug.Log("You selected an attack that hits multiple targets.");
            if (teams[targetedTeamIndex].Count < attack.damage.Length)
            {
                for (int i = 0; i <= teams[targetedTeamIndex].Count; i++)
                {
                    if (i >= attack.minRange - 1 && i <= attack.maxRange - 1)
                    {
                        eligibleTargets.Add(new List<GameObject>());
                        for (int j = 0; j < attack.damage.Length; j++)
                        {
                            if (attack.damage[j] != 0)
                            {
                                //Debug.Log("This character was in range of this AoE attack: " + team[j + i].name);
                                if((j+i) < teams[targetedTeamIndex].Count)
                                eligibleTargets[option].Add(teams[targetedTeamIndex][j + i]);
                            }
                            else
                            {
                                //Debug.Log("Character starting at position " + j + " (" + team[j + i].name + ") was in range, but the attack didn't have a damage value at this point.");
                            }
                        }
                        option++;
                    }
                    else
                    {
                        //Debug.Log("Character starting at position " + i + " was not in range.");
                    }
                }
            }
            else
            {
                for (int i = 0; i <= teams[targetedTeamIndex].Count - attack.damage.Length; i++)
                {
                    if (i >= attack.minRange - 1 && i <= attack.maxRange - 1)
                    {
                        eligibleTargets.Add(new List<GameObject>());
                        for (int j = 0; j < attack.damage.Length; j++)
                        {
                            if (attack.damage[j] != 0)
                            {
                                //Debug.Log("This character was in range of this AoE attack: " + team[j + i].name);
                                eligibleTargets[option].Add(teams[targetedTeamIndex][j + i]);
                            }
                            else
                            {
                                //Debug.Log("Character starting at position " + j + " (" + team[j + i].name + ") was in range, but the attack didn't have a damage value at this point.");
                            }
                        }
                        option++;
                    }
                    else
                    {
                        //Debug.Log("Character starting at position " + i + " was not in range.");
                    }
                }
            }
        }

        //Debug.Log("This list's length is..." + eligibleTargets.Count);
        return eligibleTargets;
    }

    public bool HasEligibleTargets(List<List<GameObject>> eligibleTargets)
    {
        return eligibleTargets.Count != 0;
    }

    public void FireGunEffects()
    {
        if (targets?.Any() == true)
        {
            LineEffects.Instance.Fire(gunBarrel.position, targets, character.weapon.element);
        }
    }
}
