using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroStateMachine : CharacterStateMachine
{
    [Header("UI")]
    public Slider atb;
    public TextMeshProUGUI healthUI;
    public TextMeshProUGUI nameUI;

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
                } else
                {
                    isAlive = false;
                    gameObject.tag = "DeadHero";

                    battleStateMachine.heroes.Remove(gameObject);
                    battleStateMachine.readyHeroes.Remove(gameObject);
                    selector.SetActive(false);
                    battleStateMachine.actionPanel.SetActive(false);
                    battleStateMachine.targetPanel.SetActive(false);
                    battleStateMachine.turnList.RemoveAll(turn => turn.attacker == gameObject.name);
                    battleStateMachine.heroInput = BattleStateMachine.HeroInputState.ACTIVATE;
                }
                break;
            default:
                break;
        }
    }

    public override void UpdateATB()
    {
        atbProgress += Time.deltaTime * character.currSpeed;
        atb.value = atbProgress;

        if(atbProgress >= 1)
        {
            currentState = TurnState.ADDTOLIST;
        }
    }

    public override void TakeDamage(float dmg)
    {
        character.currHP -= dmg;
        healthUI.text = "" + character.currHP;

        if(character.currHP <= 0)
        {
            character.currHP = 0;
            currentState = TurnState.DEAD;
        }
    }

    public void updateUI()
    {
        healthUI.text = "" + character.currHP;
        nameUI.text = character.name;
    }
}
