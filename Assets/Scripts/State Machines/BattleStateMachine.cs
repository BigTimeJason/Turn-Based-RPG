using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleStateMachine : MonoBehaviour
{
    public enum BattleState
    {
        WAIT,
        TAKEACTION,
        PERFORMACTION
    }

    public enum HeroInputState
    {
        ACTIVATE,
        WAITING,
        INPUT1,
        INPUT2,
        DONE
    }
    [Header("States")]
    public BattleState battleState;
    public HeroInputState heroInput;

    [Header("Lists")]
    public List<GameObject> readyHeroes = new List<GameObject>();
    public List<HandleTurn> turnList = new List<HandleTurn>();
    public List<GameObject> heroes = new List<GameObject>();
    public List<GameObject> enemies = new List<GameObject>();

    [Header("UI")]
    public GameObject heroPanel;
    public GameObject actionPanel;
    public GameObject targetPanel;
    public GameObject heroPanelBar;
    public GameObject actionPanelButton;
    public GameObject targetPanelButton;

    private HandleTurn heroChoice;

    void Start()
    {
        battleState = BattleState.WAIT;
        heroInput = HeroInputState.ACTIVATE;

        actionPanel.SetActive(false);
        targetPanel.SetActive(false);

        heroes.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
        enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        enemies.Sort((e1, e2) => e1.GetComponent<EnemyStateMachine>().character.slot.CompareTo(e2.GetComponent<EnemyStateMachine>().character.slot));
        heroes.Sort((h1, h2) => h1.GetComponent<HeroStateMachine>().character.slot.CompareTo(h2.GetComponent<HeroStateMachine>().character.slot));

        GenerateHeroPanels();
    }


    void Update()
    {
        switch (battleState)
        {
            case BattleState.WAIT:
                if (turnList.Count > 0)
                {
                    battleState = BattleState.TAKEACTION;
                }
                break;
            case BattleState.TAKEACTION:
                GameObject performer = GameObject.Find(turnList[0].attacker);
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

                    enemyStateMachine.targets = turnList[0].targetGameObject;
                    enemyStateMachine.currentState = EnemyStateMachine.TurnState.ACTION;

                } else if (turnList[0].type == "player" && enemies.Count > 0)
                {
                    HeroStateMachine heroStateMachine = performer.GetComponent<HeroStateMachine>();
                    heroStateMachine.targets = turnList[0].targetGameObject;
                    heroStateMachine.currentState = HeroStateMachine.TurnState.ACTION;
                }

                battleState = BattleState.PERFORMACTION;
                break;
            case BattleState.PERFORMACTION:
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
                    GenerateActionButtons(readyHeroes[0].GetComponent<HeroStateMachine>().character.availableActions);

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
                foreach (GameObject target in heroChoice.targetGameObject)
                {
                    target.transform.Find("Selector").gameObject.SetActive(false);
                }
                HeroInputDone();
                break;
            default:
                break;
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

        foreach (Transform child in actionPanel.transform.Find("Spacer").transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < characterActions.Count; i++) {
            GameObject currButton = Instantiate(actionPanelButton);
            currButton.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text = currentHero.character.availableActions[i].GetName();
            Button button = currButton.GetComponent<Button>();

            if (!currentHero.character.availableActions[i].CreatesNewMenu())
            {
                Action tempAction = new Action();
                tempAction = characterActions[i].GetAction(0);

                button.onClick.AddListener(() => Input1(tempAction)); // Doesn't work and I don't know why.
            } else
            {
                List<Action> tempActions = new List<Action>();
                tempActions = characterActions[i].GetActions();

                button.onClick.AddListener(() => GenerateActionButtons(tempActions));
            }

            currButton.transform.SetParent(actionPanel.transform.Find("Spacer"));
        }
    }

    public void GenerateActionButtons(List<Action> characterActions)
    {
        HeroStateMachine currentHero = readyHeroes[0].GetComponent<HeroStateMachine>();

        foreach (Transform child in actionPanel.transform.Find("Spacer").transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < characterActions.Count; i++)
        {
            GameObject currButton = Instantiate(actionPanelButton);
            currButton.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text = characterActions[i].attackName;
            Button button = currButton.GetComponent<Button>();

            Action tempAction = new Action();
            tempAction = characterActions[i];

            button.onClick.AddListener(() => Input1(tempAction));

            currButton.transform.SetParent(actionPanel.transform.Find("Spacer"));
        }
    }
    public void GenerateTargetsButtons()
    {
        foreach (Transform child in targetPanel.transform.Find("Spacer").transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        List<List<GameObject>> allEligibleTargets = heroChoice.attackerGameObject.GetComponent<HeroStateMachine>().GetEligibleTargets(heroChoice.attack);
        
        for (int i = 0; i < allEligibleTargets.Count; i++)
        {
            GameObject currButton = Instantiate(targetPanelButton);
            EnemySelectButton button = currButton.GetComponent<EnemySelectButton>();
            button.enemyPrefabs = allEligibleTargets[i];

            int targetNum = 0;
            currButton.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text = "";

            foreach (GameObject target in allEligibleTargets[i])
            {
                if (targetNum > 0)
                {
                    currButton.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text += "" + target.name;
                } else currButton.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text += target.name;

                targetNum++;
            }

            currButton.SetActive(true);
            currButton.transform.SetParent(targetPanel.transform.Find("Spacer"));
        }
    }

    public void Input1(Action attack)
    {
        HeroStateMachine currentHero = readyHeroes[0].GetComponent<HeroStateMachine>();
        if (currentHero.HasEligibleTargets(currentHero.GetEligibleTargets(attack)))
        {
            heroChoice.attacker = readyHeroes[0].name;
            heroChoice.attackerGameObject = readyHeroes[0];
            heroChoice.type = "player";
            heroChoice.attack = attack;

            actionPanel.SetActive(false);
            targetPanel.SetActive(true);
            GenerateTargetsButtons();
        } else
        {
            Debug.Log("This attack has no elligible targets.");
        }
    }

    public void Input2(List<GameObject> targetEnemy)
    {
        heroChoice.targetGameObject = targetEnemy;
        heroInput = HeroInputState.DONE;
    }

    public void HeroInputDone()
    {
        turnList.Add(heroChoice);
        targetPanel.SetActive(false);
        readyHeroes[0].transform.Find("Selector").gameObject.SetActive(false);
        readyHeroes.RemoveAt(0);
        heroInput = HeroInputState.ACTIVATE;
    }
}
