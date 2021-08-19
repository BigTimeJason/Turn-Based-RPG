using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleStateMachine : MonoBehaviour
{
    private static BattleStateMachine _instance;
    public static BattleStateMachine Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    public enum BattleState
    {
        PRELOAD,
        WAIT,
        PAUSED,
        TAKEACTION,
        PERFORMACTION,
        CHECKALIVE,
        WIN,
        LOSE
    }

    public enum HeroInputState
    {
        ACTIVATE,
        WAITING,
        INPUT1,
        INPUT2,
        DONE,
        ENDATTACK
    }

    [Header("Prefabs")]
    public GameObject heroPrefab;
    public GameObject enemyPrefab;

    [Header("States")]
    public BattleState battleState;
    public HeroInputState heroInput;
    [SerializeField]
    private HandleTurn heroChoice;
    public bool hasSelectedAttack;
    public bool hasEnded = false;

    [Header("Lists")]
    public List<GameObject> readyHeroes = new List<GameObject>();
    public List<HandleTurn> turnList = new List<HandleTurn>();
    public List<GameObject> heroes = new List<GameObject>();
    public List<GameObject> enemies = new List<GameObject>();
    public Transform[] heroSlots = new Transform[] { };
    public Transform[] enemySlots = new Transform[] { };

    [Header("UI")]
    public GameObject heroPanel;
    public GameObject actionPanel;
    public GameObject targetPanel;
    public GameObject heroPanelBar;
    public GameObject actionPanelButton;
    public GameObject targetPanelButton;
    public GameObject attackNameUI;
    public GameObject runButton;
    public float animTime;

    [Header("Camera")]
    public BattleCamera battleCamera;
    public bool finishedAnimation;
    public bool finishedCamera;
    public bool characterDied;

    void Start()
    {
        InitCharacters();
        InitSetup();
        InitUI();
        AnimateCharactersIn();
    }

    void InitCharacters()
    {
        //heroes = GameManager.Instance.heroes;
        foreach(Character hero in GameManager.Instance.heroes)
        {
            GameObject currHero = Instantiate(heroPrefab, GameObject.Find("Heroes").transform.position + new Vector3(6-hero.slot, 0, 0), Quaternion.identity);
            currHero.GetComponent<HeroStateMachine>().character = hero;
            if (hero.heroSpriteSheetName != null) currHero.GetComponent<HeroStateMachine>().SpriteSheetName = hero.heroSpriteSheetName;
            currHero.transform.localScale = new Vector3(1, 1, 1);
        }

        for(int i = 0; i < GameManager.Instance.enemies.Count; i++)
        {
            GameObject currEnemy = Instantiate(enemyPrefab, enemySlots[i]);
            currEnemy.GetComponent<EnemyStateMachine>().character = GameManager.Instance.enemies[i];
            currEnemy.transform.localScale = new Vector3(-1, 1, 1);

        }
    }

    void InitSetup()
    {
        battleState = BattleState.PRELOAD;
        heroInput = HeroInputState.ACTIVATE;

        heroes.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
        enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        heroes.Sort((h1, h2) => h1.GetComponent<HeroStateMachine>().character.slot.CompareTo(h2.GetComponent<HeroStateMachine>().character.slot));
        enemies.Sort((e1, e2) => e1.GetComponent<EnemyStateMachine>().character.slot.CompareTo(e2.GetComponent<EnemyStateMachine>().character.slot));
    }

    void InitUI()
    {
        GenerateHeroPanels();
        actionPanel.SetActive(false);
        targetPanel.SetActive(false);
        //runButton = GameObject.Find("RunButton"); todo
    }

    void AnimateCharactersIn()
    {
        StartCoroutine(Co_AnimateCharactersIn());
    }

    IEnumerator Co_AnimateCharactersIn()
    {
        foreach (GameObject hero in heroes)
        {
            hero.transform.position = hero.transform.position - new Vector3(5, 0, 0);
        }
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<EnemyStateMachine>().InitBattle();
        }
        foreach (GameObject hero in heroes)
        {
            hero.transform.DOMoveX(hero.transform.position.x + 5, 0.5f).OnComplete(() =>
            {
                hero.GetComponent<HeroStateMachine>().InitBattle();
            });
            yield return new WaitForSeconds(0.5f);
        }
        battleState = BattleState.WAIT;
        //UpdateCharacterPositions();
        InitCamera();
    }

    void InitCamera()
    {
        battleCamera = FindObjectOfType<BattleCamera>();
        battleCamera.Init();

        foreach (GameObject hero in heroes)
        {
            battleCamera.AddMember(hero);
        }
        foreach (GameObject enemy in enemies)
        {
            battleCamera.AddMember(enemy);
        }
    }

    void Update()
    {
        switch (battleState)
        {
            case BattleState.PRELOAD:
                //battleState = BattleState.WAIT;
                break;
            case BattleState.WAIT:
                //battleCamera.ResetTargets();
                if (turnList.Count > 0)
                {
                    battleState = BattleState.TAKEACTION;
                }
                break;
            case BattleState.PAUSED:
                break;
            case BattleState.TAKEACTION:
                GameObject performer = GameObject.Find(turnList[0].attacker);
                //if(turnList[0].targetGameObjects.Any)

                if(turnList[0].type == "enemy" && heroes.Count > 0)
                {
                    EnemyStateMachine enemyStateMachine = performer.GetComponent<EnemyStateMachine>();

                    //If the target dies after the command is selected but before it goes off.
                    //if (enemyStateMachine.targets.Count == 1)
                    //{
                    //    if (!heroes.Contains(enemyStateMachine.targets[0]))
                    //    {
                    //        turnList[0].targetGameObject = enemyStateMachine.PickTargetFromEligibleTargets(enemyStateMachine.GetEligibleTargets(turnList[0].attack));
                    //    }
                    //}

                    enemyStateMachine.targets = turnList[0].targetGameObjects;
                    enemyStateMachine.currentState = EnemyStateMachine.TurnState.ACTION;

                } else if (turnList[0].type == "player" && enemies.Count > 0)
                {
                    HeroStateMachine heroStateMachine = performer.GetComponent<HeroStateMachine>();
                    heroStateMachine.targets = turnList[0].targetGameObjects;
                    heroStateMachine.currentState = HeroStateMachine.TurnState.ACTION;
                }

                //battleCamera.FocusTwo(performer, turnList[0].targetGameObject);
                StartCoroutine(TargetsUIFocus(performer, turnList[0]));

                battleState = BattleState.PERFORMACTION;
                break;
            case BattleState.PERFORMACTION:
                if (finishedAnimation && finishedCamera)
                {
                    finishedCamera = false;
                    finishedAnimation = false;
                    if (characterDied)
                    {
                        Debug.Log("Character Died");
                        characterDied = false;
                        battleState = BattleState.CHECKALIVE;
                    }
                    else
                    {
                        if (!hasSelectedAttack)
                        { 
                            heroInput = HeroInputState.ACTIVATE;
                        }
                        battleState = BattleState.WAIT;
                    }
                }

                break;
            case BattleState.CHECKALIVE:
                //Debug.Log(heroes.Count);
                if(heroes.Count < 1)
                {
                    battleState = BattleState.LOSE;
                } else if(enemies.Count < 1)
                {
                    battleState = BattleState.WIN;
                } else
                {
                    //ClearPanels();
                    if (hasSelectedAttack)
                    {
                        hasSelectedAttack = false;
                    }
                    else
                    {
                        //heroInput = HeroInputState.ACTIVATE;
                    }
                    battleState = BattleState.WAIT;
                }
                break;
            case BattleState.WIN:
                Debug.Log("You Won");
                foreach(GameObject hero in heroes)
                {
                    hero.GetComponent<HeroStateMachine>().currentState = CharacterStateMachine.TurnState.WON;
                }
                if (!hasEnded)
                {
                    hasEnded = true;
                    StartCoroutine(WonBattleCoroutine());
                }
                break;
            case BattleState.LOSE:
                Debug.Log("You Lost"); if (!hasEnded)
                {
                    hasEnded = true;
                    StartCoroutine(LostBattleCoroutine());
                }
                break;
            default:
                break;
        }

        switch (heroInput)
        {
            case HeroInputState.ACTIVATE:
                if(readyHeroes.Count > 0)
                {
                    readyHeroes[0].transform.Find("Selector").gameObject.SetActive(true);
                    heroChoice = new HandleTurn();

                    actionPanel.SetActive(true);
                    DestroyActionButtons();
                    actionPanel.transform.localScale = new Vector3(1, 0);
                    actionPanel.transform.DOScaleY(1, animTime).OnComplete(() => {
                        GenerateActionButtons(readyHeroes[0].GetComponent<HeroStateMachine>().character.availableActions);

                    });

                    heroInput = HeroInputState.WAITING;
                }
                break;
            case HeroInputState.WAITING:
                break;
            case HeroInputState.INPUT1:
                break;
            case HeroInputState.INPUT2:
                break;
            case HeroInputState.DONE:
                foreach (GameObject target in heroChoice.targetGameObjects)
                {
                    target.transform.Find("Targeter").gameObject.SetActive(false);
                }
                HeroInputDone();
                break;
            case HeroInputState.ENDATTACK:
                break;
            default:
                break;
        }
    }

    public IEnumerator TargetsUIFocus(GameObject performer, HandleTurn action)
    {
        battleCamera.AddFocus(performer);
        attackNameUI.GetComponentInChildren<TextMeshProUGUI>().SetText(action.attack.actionName);
        attackNameUI.SetActive(true);
        yield return new WaitForSeconds(1f);
        foreach (GameObject target in action.targetGameObjects)
        {
            battleCamera.AddFocus(target);
        }
        yield return new WaitForSeconds(2f);
        attackNameUI.SetActive(false);
        battleCamera.ResetTargets();
        finishedCamera = true;
    }

    public void CharacterFinishedAnimation()
    {
        finishedAnimation = true;
    }

    public void CharacterDied(GameObject character)
    {
        characterDied = true;
        foreach (HandleTurn action in turnList.ToList())
        {
            foreach(GameObject target in action.targetGameObjects.ToList())
            {
                if (target.name == character.name)
                {
                    action.targetGameObjects.Remove(target);
                }
            }
        }

        CharacterStateMachine characterStateMachine = character.GetComponent<CharacterStateMachine>();
        if (character.CompareTag("DeadHero"))
        {
            int startingSlot = characterStateMachine.character.slot;
            if (startingSlot < heroes.Count) {
                for (int i = startingSlot; i < heroes.Count; i++)
                {
                    heroes[i].GetComponent<CharacterStateMachine>().character.slot -= 1;
                }
            }
            if (readyHeroes.Count != 0)
            {
                if (readyHeroes[0] == character)
                {
                    Debug.Log("Cleared Panels because the active character died!");
                    ClearPanels();
                    hasSelectedAttack = false;
                    heroInput = HeroInputState.ACTIVATE;

                    //if (hasSelectedAttack)
                    //{
                    //    hasSelectedAttack = false;

                    //    //TODO remove selector
                    //    //heroChoice.targetGameObjects
                    //}
                }
            }
            heroes.Remove(character);
            readyHeroes.Remove(character);
            turnList.RemoveAll(turn => turn.attacker == character.name);

            character.GetComponent<SpriteRenderer>().DOColor(new Color(0, 0, 0, 0), 1).OnComplete(() =>
            {
                character.SetActive(false);
            });
        } else if (character.CompareTag("DeadEnemy"))
        {
            character.GetComponent<SpriteRenderer>().DOColor(new Color(0, 0, 0, 0), 1).OnComplete(() =>
            {
                character.SetActive(false);
            });

            int startingSlot = characterStateMachine.character.slot;
            if (startingSlot < enemies.Count)
            {
                for (int i = startingSlot; i < enemies.Count; i++)
                {
                    enemies[i].GetComponent<CharacterStateMachine>().character.slot -= 1;
                }
            }
            if (hasSelectedAttack) GenerateTargetButtons();
        }
    }

    public void UpdateCharacterPositions()
    {
        heroes.Sort((h1, h2) => h1.GetComponent<HeroStateMachine>().character.slot.CompareTo(h2.GetComponent<HeroStateMachine>().character.slot));
        enemies.Sort((e1, e2) => e1.GetComponent<EnemyStateMachine>().character.slot.CompareTo(e2.GetComponent<EnemyStateMachine>().character.slot));

        for (int i = 0; i < heroes.Count; i++)
        {
            heroes[i].transform.DOMove(heroSlots[i].transform.position, 1);
            //heroes[i].GetComponent<CharacterStateMachine>().MoveTowards(heroSlots[i].transform.position);
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].transform.DOMove(enemySlots[i].transform.position, 1);
            //enemies[i].GetComponent<CharacterStateMachine>().MoveTowards(enemySlots[i].transform.position);
        }
    }

    public void AddAction(HandleTurn input)
    {
        turnList.Add(input);
    }

    public void GenerateHeroPanels()
    {
        foreach (Transform child in heroPanel.transform.Find("Spacer").transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < heroes.Count; i++)
        {
            GameObject currHero = Instantiate(heroPanelBar);
            //currHero.transform.localScale = new Vector3(1, 0);
            //currHero.transform.DOScaleY(1, 1);
            HeroStateMachine hero = heroes[i].GetComponent<HeroStateMachine>();

            hero.nameUI = currHero.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            hero.atb = currHero.transform.Find("ATBBar").GetComponent<Slider>();
            hero.healthUI = currHero.transform.Find("HP").GetComponent<TextMeshProUGUI>();
            hero.UpdateUI();

            currHero.SetActive(true);
            currHero.transform.SetParent(heroPanel.transform.Find("Spacer"));
        }
    }

    public void GenerateActionButtons(List<CharacterAction> characterActions)
    {
        HeroStateMachine currentHero = readyHeroes[0].GetComponent<HeroStateMachine>();

        DestroyActionButtons();

        for (int i = 0; i < characterActions.Count; i++) {
            GameObject currButton = Instantiate(actionPanelButton);
            currButton.transform.localScale = new Vector3(0, 1);
            currButton.transform.DOScaleX(1, animTime);
            //currButton.transform.DOLocalMoveY(5, 1);
            currButton.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text = currentHero.character.availableActions[i].GetName();
            Button button = currButton.GetComponent<Button>();

            if (!currentHero.character.availableActions[i].CreatesNewMenu())
            {
                Action tempAction = characterActions[i].GetAction(0);

                button.onClick.AddListener(() => Input1(tempAction));
                button.GetComponent<TooltipTrigger>().header = tempAction.actionName;
                button.GetComponent<TooltipTrigger>().content = tempAction.description;
            } else
            {
                List<Action> tempActions = characterActions[i].GetActions();

                button.onClick.AddListener(() => GenerateActionButtons(tempActions));
                button.GetComponent<TooltipTrigger>().header = characterActions[i].name;
                button.GetComponent<TooltipTrigger>().content = "Opens the menu for " + characterActions[i].name;
            }

            currButton.transform.SetParent(actionPanel.transform.Find("Spacer"));
        }
    }

    public void GenerateActionButtons(List<Action> characterActions)
    {
        HeroStateMachine currentHero = readyHeroes[0].GetComponent<HeroStateMachine>();

        DestroyActionButtons();

        for (int i = 0; i < characterActions.Count; i++)
        {
            GameObject currButton = Instantiate(actionPanelButton);
            currButton.transform.localScale = new Vector3(0, 1);
            currButton.transform.DOScaleX(1, animTime);
            //currButton.transform.DOLocalMoveY(5, 1);
            currButton.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text = characterActions[i].actionName;
            Button button = currButton.GetComponent<Button>();

            Action tempAction = characterActions[i];

            button.onClick.AddListener(() => Input1(tempAction));
            button.GetComponent<TooltipTrigger>().header = tempAction.actionName;
            button.GetComponent<TooltipTrigger>().content = tempAction.description;

            currButton.transform.SetParent(actionPanel.transform.Find("Spacer"));
        }
    }

    public void DestroyActionButtons()
    {
        foreach (Transform child in actionPanel.transform.Find("Spacer").transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void ClearPanels()
    {
        targetPanel.SetActive(false);
        actionPanel.SetActive(false);

        foreach (GameObject enemy in enemies)
        {
            //Debug.Log("Cleared targeter from " + enemy.name);
            enemy.transform.Find("Selector").gameObject.SetActive(false);
            enemy.transform.Find("Targeter").gameObject.SetActive(false);
        }
        foreach (GameObject hero in heroes)
        {
           //Debug.Log("Cleared targeter from " + hero.name);
            hero.transform.Find("Targeter").gameObject.SetActive(false);
        }

        TooltipSystem.Hide();
    }

    public void GenerateTargetButtons()
    {
        DestroyTargetButtons();

        List<List<GameObject>> allEligibleTargets = heroChoice.attackerGameObject.GetComponent<HeroStateMachine>().GetEligibleTargets(heroChoice.attack);

        for (int i = 0; i < allEligibleTargets.Count; i++)
        {
            GameObject currButton = Instantiate(targetPanelButton);
            currButton.transform.localScale = new Vector3(0, 1);
            currButton.transform.DOScaleX(1, animTime);
            EnemySelectButton button = currButton.GetComponent<EnemySelectButton>();
            button.enemyPrefabs = allEligibleTargets[i];

            int targetNum = 0;
            currButton.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text = "";

            foreach (GameObject target in allEligibleTargets[i])
            {
                if (targetNum > 0)
                {
                    currButton.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text += ", " + target.name;
                }
                else currButton.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text += target.name;

                targetNum++;
            }

            currButton.SetActive(true);
            currButton.transform.SetParent(targetPanel.transform.Find("Spacer"));
        }
    }

    public void DestroyTargetButtons()
    {
        foreach (Transform child in targetPanel.transform.Find("Spacer").transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void Input1(Action attack)
    {
        heroInput = HeroInputState.INPUT1;
        HeroStateMachine currentHero = readyHeroes[0].GetComponent<HeroStateMachine>();
        if (currentHero.HasEligibleTargets(currentHero.GetEligibleTargets(attack)))
        {
            heroChoice.attacker = readyHeroes[0].name;
            heroChoice.attackerGameObject = readyHeroes[0];
            heroChoice.type = "player";
            heroChoice.attack = attack;

            actionPanel.transform.DOScaleY(0, animTime).OnComplete(() =>
            {
                actionPanel.SetActive(false);
            });
            targetPanel.SetActive(true);
            DestroyTargetButtons();
            targetPanel.transform.localScale = new Vector3(1, 0);
            targetPanel.transform.DOScaleY(1, animTime).OnComplete(() =>
            {
                GenerateTargetButtons();
            });
            hasSelectedAttack = true;
        } else
        {
            Debug.Log("This attack has no eligible targets.");
        }
    }

    public void Input2(List<GameObject> targetEnemy)
    {
        heroInput = HeroInputState.INPUT2;
        heroChoice.targetGameObjects = targetEnemy;
        heroInput = HeroInputState.DONE;
    }

    public void StartRun()
    {
        foreach(GameObject hero in heroes)
        {
            hero.GetComponent<HeroStateMachine>().StartRun();
        }
    }
    public void EndRun()
    {
        foreach (GameObject hero in heroes)
        {
            hero.GetComponent<HeroStateMachine>().EndRun();
        }
    }

    public void Push(GameObject character, int pushed)
    {
        if (character.CompareTag("Hero"))
        {
            CharacterStateMachine currChar = character.GetComponent<CharacterStateMachine>();
            int i = currChar.character.slot + pushed;
            while (i >= heroes.Count)
            {
                i--;
            }
            int tempSlot = currChar.character.slot;
            currChar.character.slot = heroes[i].GetComponent<CharacterStateMachine>().character.slot;
            heroes[i].GetComponent<CharacterStateMachine>().character.slot = tempSlot;
        }
        else if (character.CompareTag("Enemy"))
        {
            CharacterStateMachine currChar = character.GetComponent<CharacterStateMachine>();
            int i = currChar.character.slot + pushed;
            while (i >= enemies.Count)
            {
                i--;
            }
            int tempSlot = currChar.character.slot;
            currChar.character.slot = heroes[i].GetComponent<CharacterStateMachine>().character.slot;
            enemies[i].GetComponent<CharacterStateMachine>().character.slot = tempSlot;
        }
        UpdateCharacterPositions();
    }

    public void Swap()
    {

    }

    public void HeroInputDone()
    {
        turnList.Add(heroChoice);
        ClearPanels();
        heroInput = HeroInputState.ACTIVATE;
        hasSelectedAttack = false;
        readyHeroes[0].transform.Find("Selector").gameObject.SetActive(false);
        readyHeroes.RemoveAt(0);
    }

    IEnumerator WonBattleCoroutine()
    {
        SoundManager.Instance.PlayMusic(3);
        yield return new WaitForSeconds(5f);
        SoundManager.Instance.PlayMusic(1);

        

        GameManager.Instance.FinishedMission();
        GameManager.Instance.ResetCharacters();
        LevelLoader.Instance.LoadScene("LobbyScene");
    }

    IEnumerator LostBattleCoroutine()
    {
        SoundManager.Instance.StopMusic();
        attackNameUI.GetComponentInChildren<TextMeshProUGUI>().SetText("Game Over...");
        attackNameUI.SetActive(true);
        yield return new WaitForSeconds(5f);
        GameManager.Instance.ResetCharacters();
        attackNameUI.SetActive(false);
        LevelLoader.Instance.LoadScene("Preload");
    }
}
