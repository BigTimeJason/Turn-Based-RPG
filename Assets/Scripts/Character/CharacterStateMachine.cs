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
    public SpriteRenderer spriteRenderer;

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
        spriteRenderer = GetComponent<SpriteRenderer>();
        battleStateMachine = BattleStateMachine.Instance;
        gameObject.name = character.charName;
        atbProgress = Random.Range(0, character.BaseSpeed * 2);
        animator = GetComponent<Animator>();
    }

    public virtual void InitBattle()
    {
        selector.SetActive(false);
        currentState = TurnState.PROCESSING;
        startPosition = transform.position;
        if (character.enemySprite != null)
        {
            GetComponent<SpriteRenderer>().sprite = character.enemySprite;
        }

        spriteRenderer.material.SetColor("_Colour", GetColours.GetColourOfElement(character.ShieldElement));
        if (character.CurrShieldHP > 0)
        {
            spriteRenderer.material.SetFloat("_Outline_Thickness", 1);
        }
        else
        {
            spriteRenderer.material.SetFloat("_Outline_Thickness", 0);
        }
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
        atbProgress += Time.deltaTime * character.CurrSpeed;

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
        if (character.CurrShieldHP > 0)
        {
            //dmg /= character.CurrElemDef + 1;
            if (character.ShieldElement == element)
            {
                dmg = dmg * 2;
                character.CurrShieldHP -= dmg;
                if (character.CurrShieldHP <= 0)
                {
                    character.CurrShieldHP = 0;
                    ParticleSystem.MainModule settings = shieldParticles.main;
                    settings.startColor = new ParticleSystem.MinMaxGradient(GetColours.GetColourOfElement(element));
                    shieldParticles.Play();
                }
            } else
            {
                character.CurrShieldHP -= dmg * 0.5f;
                if (character.CurrShieldHP <= 0)
                {
                    character.CurrShieldHP = 0;
                }
            }
        }
        else
        {
            character.CurrHP -= dmg;
            if (character.CurrHP > character.MaxHP)
            {
                character.CurrHP = character.MaxHP;
            }
        }

        DamageNumbers.Instance.Show(dmg, this.gameObject);

        if (character.CurrHP <= 0)
        {
            character.CurrHP = 0;
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
                float dmg = battleStateMachine.turnList[0].attack.damage[i] * (character.CurrPower) + (Random.Range(-character.CurrPower/ 10, character.CurrPower / 10));
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
            // If the attack has more targets than there are players on that team.
            if (teams[targetedTeamIndex].Count < attack.damage.Length)
            {
                for (int i = 0; i <= teams[targetedTeamIndex].Count; i++)
                {
                    if (i >= attack.minRange - 1 && i < attack.maxRange)
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
                    if (i >= attack.minRange - 1 && i < attack.maxRange)
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
            if (BattleStateMachine.Instance.turnList[0].attack.clip != null)
            {
                SoundManager.Instance.Play(BattleStateMachine.Instance.turnList[0].attack.clip);
            } else if (character.weapon.gunSound != null)
            {
                SoundManager.Instance.Play(character.weapon.gunSound);
            }
            LineEffects.Instance.Fire(gunBarrel.position, targets, character.weapon.element);
        }
    }
}
