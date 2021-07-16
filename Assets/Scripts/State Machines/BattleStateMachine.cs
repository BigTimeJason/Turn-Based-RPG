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

    public BattleState battleState;
    public HeroInputState heroInput;

    public List<GameObject> readyHeroes = new List<GameObject>();
    public List<HandleTurn> turnList = new List<HandleTurn>();
    public List<GameObject> heroes = new List<GameObject>();
    public List<GameObject> enemies = new List<GameObject>();
    public List<GameObject> enemyTargetButtons = new List<GameObject>();
    public List<GameObject> heroUI = new List<GameObject>();

    public GameObject attackPanel;
    public GameObject targetPanel;

    private HandleTurn heroChoice;

    void Start()
    {
        battleState = BattleState.WAIT;
        heroInput = HeroInputState.ACTIVATE;

        attackPanel.SetActive(false);
        targetPanel.SetActive(false);

        heroes.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
        enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        enemies.Sort((e1, e2) => e1.GetComponent<EnemyStateMachine>().character.slot.CompareTo(e2.GetComponent<EnemyStateMachine>().character.slot));
        heroes.Sort((h1, h2) => h1.GetComponent<HeroStateMachine>().character.slot.CompareTo(h2.GetComponent<HeroStateMachine>().character.slot));
        heroUI.Sort((h1, h2) => h1.gameObject.name.CompareTo(h2.gameObject.name));

        for (int i = 0; i < heroes.Count && i < heroUI.Count; i++)
        {
            HeroStateMachine hero = heroes[i].GetComponent<HeroStateMachine>();
            hero.atb = heroUI[i].transform.Find("ATBBar").GetComponent<Slider>();
            hero.healthUI = heroUI[i].transform.Find("HP").GetComponent<TextMeshProUGUI>();
            hero.nameUI = heroUI[i].transform.Find("Name").GetComponent<TextMeshProUGUI>();
            hero.updateUI();

            heroUI[i].SetActive(true);
            
        }
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

                    attackPanel.SetActive(true);
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

    public void EnemyButtons()
    {
        foreach(GameObject enemyButton in enemyTargetButtons)
        {
            enemyButton.SetActive(false);
        }

        List<List<GameObject>> allEligibleTargets = heroChoice.attackerGameObject.GetComponent<HeroStateMachine>().GetEligibleTargets(heroChoice.attack);
        
        for (int i = 0; i < allEligibleTargets.Count && i < enemyTargetButtons.Count; i++)
        {
            List<GameObject> currentButtonTargets = allEligibleTargets[i];

            GameObject currButton = enemyTargetButtons[i];

            EnemySelectButton button = currButton.GetComponent<EnemySelectButton>();
            currButton.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text = "";

            button.enemyPrefabs = currentButtonTargets;

            int targetNum = 0;
            foreach(GameObject target in currentButtonTargets)
            {
                if (targetNum > 0)
                {
                    currButton.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text += ", " + target.name;
                } else currButton.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text += target.name;

                targetNum++;
            }

            currButton.SetActive(true);
        }
    }

    public void Input1()
    {
        HeroStateMachine currentHero = readyHeroes[0].GetComponent<HeroStateMachine>();
        if (currentHero.HasEligibleTargets(currentHero.GetEligibleTargets(currentHero.character.availableAttacks[0])))
        {
            heroChoice.attacker = readyHeroes[0].name;
            heroChoice.attackerGameObject = readyHeroes[0];
            heroChoice.type = "player";
            heroChoice.attack = readyHeroes[0].GetComponent<HeroStateMachine>().character.availableAttacks[0];

            attackPanel.SetActive(false);
            targetPanel.SetActive(true);
            EnemyButtons();
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
