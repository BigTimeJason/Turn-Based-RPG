using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyStateMachine : CharacterStateMachine
{
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

    void ChooseAction()
    {
        int selection = Random.Range(0, character.availableActions.Count);
        Action currentAttack = character.availableActions[selection].GetAction(Random.Range(0, character.availableActions[selection].GetActions().Count));
        if (GetEligibleTargets(currentAttack).Count == 0)
        {
            return;
        }
        HandleTurn attack = new HandleTurn(character.name, "enemy", gameObject, PickTargetFromEligibleTargets(GetEligibleTargets(currentAttack)), currentAttack);
        battleStateMachine.AddAction(attack);
    }
}
