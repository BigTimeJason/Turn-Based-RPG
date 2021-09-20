using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character
{
    public string charName;
    public CharacterStatSheet characterStatSheet;
    public int slot;
    public int xp;
    public string heroSpriteSheetName;
    public Sprite enemySprite;
    public Sprite characterArt;

    [SerializeField]
    private Element offenseElement;
    private float basePower;
    [SerializeField]
    private float currPower;

    private float maxHP;
    [SerializeField]
    private float currHP;

    [SerializeField]
    private Element shieldElement;
    private float maxShieldHP;
    [SerializeField]
    private float currShieldHP;

    private float baseSpeed;
    [SerializeField]
    private float currSpeed;

    private float baseDef;
    [SerializeField]
    private float currDef;

    private float baseElemDef;
    [SerializeField]
    private float currElemDef;

    private float baseLuck;
    [SerializeField]
    private float currLuck;

    public BaseWeapon weapon;
    public List<CharacterAction> availableActions = new List<CharacterAction>();

    public Element OffenseElement { get => offenseElement; set => offenseElement = value; }
    public float BasePower { get => basePower; set => basePower = value; }
    public float CurrPower { get => currPower; set => currPower = value; }
    public float MaxHP { get => maxHP; set => maxHP = value; }
    public float CurrHP { get => currHP; set => currHP = value; }
    public Element ShieldElement { get => shieldElement; set => shieldElement = value; }
    public float MaxShieldHP { get => maxShieldHP; set => maxShieldHP = value; }
    public float CurrShieldHP { get => currShieldHP; set => currShieldHP = value; }
    public float BaseSpeed { get => baseSpeed; set => baseSpeed = value; }
    public float CurrSpeed { get => currSpeed; set => currSpeed = value; }
    public float BaseDef { get => baseDef; set => baseDef = value; }
    public float CurrDef { get => currDef; set => currDef = value; }
    public float BaseElemDef { get => baseElemDef; set => baseElemDef = value; }
    public float CurrElemDef { get => currElemDef; set => currElemDef = value; }
    public float BaseLuck { get => baseLuck; set => baseLuck = value; }
    public float CurrLuck { get => currLuck; set => currLuck = value; }

    public void AddAction(CharacterAction action, string name)
    {
        availableActions.Add(action);
        availableActions[availableActions.Count - 1].name = name;
    }

    // Use this if you know the character has an XP csv
    public Character(string name, CharacterStatSheet characterStatSheet, int xp, Element shieldElement = Element.KINETIC, Element offensiveElement = Element.ARC)
    {
        charName = name;
        this.xp = xp;
        this.characterStatSheet = characterStatSheet;
        this.ShieldElement = shieldElement;
        OffenseElement = offensiveElement;
        InitCharacterStats();
    }

    public Character(string name, CharacterStatSheet characterStatSheet, int slot, float power, float hp, float speed, float shield = 0, Element shieldElement = Element.KINETIC, Element offenseElement = Element.ARC)
    {
        charName = name;
        this.characterStatSheet = characterStatSheet;
        this.slot = slot;
        BasePower = power;
        CurrPower = power;
        MaxHP = hp;
        CurrHP = hp;
        MaxShieldHP = shield;
        CurrShieldHP = shield;
        BaseSpeed = speed;
        CurrSpeed = speed;
        this.ShieldElement = shieldElement;
        this.OffenseElement = offenseElement;
    }

    public void InitCharacterStats()
    {
        BasePower = float.Parse(CSVReader.GetStatsFromLevel(characterStatSheet, xp / 100)[1]);
        CurrPower = float.Parse(CSVReader.GetStatsFromLevel(characterStatSheet, xp / 100)[1]);

        MaxHP = float.Parse(CSVReader.GetStatsFromLevel(characterStatSheet, xp / 100)[2]);
        CurrHP = float.Parse(CSVReader.GetStatsFromLevel(characterStatSheet, xp / 100)[2]);

        MaxShieldHP = float.Parse(CSVReader.GetStatsFromLevel(characterStatSheet, xp / 100)[3]);
        CurrShieldHP = float.Parse(CSVReader.GetStatsFromLevel(characterStatSheet, xp / 100)[3]);

        BaseSpeed = float.Parse(CSVReader.GetStatsFromLevel(characterStatSheet, xp / 100)[4]);
        CurrSpeed = float.Parse(CSVReader.GetStatsFromLevel(characterStatSheet, xp / 100)[4]);

        BaseDef = float.Parse(CSVReader.GetStatsFromLevel(characterStatSheet, xp / 100)[5]);
        CurrDef = float.Parse(CSVReader.GetStatsFromLevel(characterStatSheet, xp / 100)[5]);

        BaseElemDef = float.Parse(CSVReader.GetStatsFromLevel(characterStatSheet, xp / 100)[6]);
        CurrElemDef = float.Parse(CSVReader.GetStatsFromLevel(characterStatSheet, xp / 100)[6]);

        BaseLuck = float.Parse(CSVReader.GetStatsFromLevel(characterStatSheet, xp / 100)[7]);
        CurrLuck = float.Parse(CSVReader.GetStatsFromLevel(characterStatSheet, xp / 100)[7]);
    }
}
