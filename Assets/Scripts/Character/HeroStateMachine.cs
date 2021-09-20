using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class HeroStateMachine : CharacterStateMachine
{
    public string SpriteSheetName;
    private string LoadedSpriteSheetName;
    private Dictionary<string, Sprite> spriteSheet;

    [Header("UI")]
    public Slider atb;
    public TextMeshProUGUI healthUI;
    public TextMeshProUGUI nameUI;

    public bool isRunning;

    public override void InitBattle()
    {
        if (animator != null)
        {
            animator.Play("Idle");
        }
        base.InitBattle();
    }

    void Update()
    {
        switch (currentState)
        {
            case TurnState.PROCESSING:
                //if (battleStateMachine.battleState == BattleStateMachine.BattleState.PRELOAD) UpdateATBUI();
                if (battleStateMachine.battleState == BattleStateMachine.BattleState.WAIT) UpdateATB();

                if (isRunning)
                {
                    if(animator != null)
                    {

                    }
                }
                break;
            case TurnState.ADDTOLIST:
                battleStateMachine.readyHeroes.Add(this.gameObject);
                currentState = TurnState.WAITING;
                if (isRunning)
                {
                    if (animator != null)
                    {

                    }
                }
                break;
            case TurnState.WAITING:
                if (isRunning)
                {
                    if (animator != null)
                    {

                    }
                } else if (animator != null)
                {
                    //animator.Play("Ready");
                }
                break;
            case TurnState.SELECTING:
                break;
            case TurnState.ACTION:
                if (animator != null)
                {
                    animator.Play(battleStateMachine.turnList[0].attack.characterAnimation);
                }
                StartCoroutine(TimeForAction());
                break;
            case TurnState.DEAD:
                if (!isAlive)
                {
                    return;
                }
                else
                {
                    if (animator != null)
                    {
                        animator.Play("Dead");
                    }
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
            case TurnState.WON:
                if (animator != null)
                {
                    animator.Play("Victory");
                }
                break;
            default:
                break;
        }
    }

    public override void UpdateATB()
    {
        atbProgress += Time.deltaTime * character.CurrSpeed;
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
        base.TakeDamage(dmg, element);
        StartCoroutine(TakeDamageAnimation(dmg));
        healthUI.text = "" + character.CurrHP;
    }

    IEnumerator TakeDamageAnimation(float dmg)
    {
        if (animator != null && dmg > 0)
        {
            animator.Play("Hurt");
        }
        yield return new WaitForSeconds(1f);
        if (animator != null)
        {
            animator.Play("Idle");
        }
    }

    public void UpdateUI()
    {
        healthUI.text = "" + character.CurrHP;
        nameUI.text = character.charName;
    }

    public void StartRun()
    {
        isRunning = true;
    }

    public void EndRun()
    {
        isRunning = false;
    }

    private void LateUpdate()
    {
        if (LoadedSpriteSheetName != SpriteSheetName && SpriteSheetName != null)
        {
            LoadSpriteSheet();
        }

        //Debug.Log(spriteRenderer.sprite.name);
        spriteRenderer.sprite = spriteSheet[spriteRenderer.sprite.name];
    }

    private void LoadSpriteSheet()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Character/" + SpriteSheetName);
        spriteSheet = sprites.ToDictionary(x => x.name, x => x);

        //Debug.Log(spriteSheet.TryGetValue(spriteRenderer.sprite.name, out debug));
        LoadedSpriteSheetName = SpriteSheetName;
    }
}
