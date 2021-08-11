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

    public override void InitBattle()
    {
        selector.SetActive(false);
        currentState = TurnState.PROCESSING;
        startPosition = transform.position;

        Action meleeAttack = Action.CreateInstance<Action>();
        meleeAttack.Init(TargetType.ENEMY, character.offenseElement, "Melee", "A melee attack", new float[] { 1.2f }, 1, 1);
        character.availableActions.Add(new CharacterAction(new List<Action>() { meleeAttack }));

        if (character.weapon != null) character.AddAction(new CharacterAction(new List<Action>() { character.weapon.weaponAttack }), character.weapon.weaponAttack.actionName);
    }

    void Update()
    {
        switch (currentState)
        {
            case TurnState.PROCESSING:
                //if (battleStateMachine.battleState == BattleStateMachine.BattleState.PRELOAD) UpdateATBUI();
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

                    atbProgress = 0;
                    atb.value = 0f;
                    selector.SetActive(false);
                    targeter.SetActive(false);
                    //battleStateMachine.actionPanel.SetActive(false);
                    //battleStateMachine.targetPanel.SetActive(false);
                    //battleStateMachine.heroes.Remove(gameObject);
                    //battleStateMachine.readyHeroes.Remove(gameObject);
                    //battleStateMachine.turnList.RemoveAll(turn => turn.attacker == gameObject.name);
                    battleStateMachine.CharacterDied(gameObject);
                    battleStateMachine.UpdateCharacterPositions();
                    //battleStateMachine.battleState = BattleStateMachine.BattleState.CHECKALIVE;
                }
                break;
            default:
                break;
        }
    }

    public override void UpdateATB()
    {
        atbProgress += Time.deltaTime * character.currSpeed;
        UpdateATBUI();

        if(atbProgress >= 100)
        {
            currentState = TurnState.ADDTOLIST;
        }
    }

    public void UpdateATBUI()
    {
        atb.value = atbProgress / 100;
    }

    public override void TakeDamage(float dmg, Element element)
    {
        if (character.shieldCurrHP > 0)
        {
            if (character.shieldElement == element)
            {
                character.shieldCurrHP -= dmg * 2;
            }

            if (character.shieldCurrHP <= 0)
            {
                character.shieldCurrHP = 0;
            }
        }
        else
        {
            character.currHP -= dmg;
        }

        if (character.currHP <= 0)
        {
            character.currHP = 0;
            currentState = TurnState.DEAD;
        }
        healthUI.text = "" + character.currHP;
    }

    public void UpdateUI()
    {
        healthUI.text = "" + character.currHP;
        nameUI.text = character.name;
    }
}
