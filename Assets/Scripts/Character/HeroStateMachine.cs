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

    private SpriteRenderer spriteRenderer;
    

    [Header("UI")]
    public Slider atb;
    public TextMeshProUGUI healthUI;
    public TextMeshProUGUI nameUI;

    public bool isRunning;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        battleStateMachine = BattleStateMachine.Instance;
        gameObject.name = character.charName;
        atbProgress = Random.Range(0, character.BaseSpeed);
        animator = GetComponent<Animator>();

    }

    public override void InitBattle()
    {
        //this.LoadSpriteSheet();
        if (animator != null)
        {
            animator.Play("Idle");
        }
        selector.SetActive(false);
        currentState = TurnState.PROCESSING;
        startPosition = transform.position;

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
                    animator.Play("Ready");
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
        if (character.CurrShieldHP > 0)
        {
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
            }
            else
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
        StartCoroutine(TakeDamageAnimation(dmg));

        DamageNumbers.Instance.Show(dmg, this.gameObject);

        if (character.CurrHP <= 0)
        {
            character.CurrHP = 0;
            currentState = TurnState.DEAD;
        }
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
