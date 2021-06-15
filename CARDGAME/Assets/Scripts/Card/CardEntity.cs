using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Create Card Entity")]
//カードデータそのもの
public class CardEntity : ScriptableObject
{
    public new string name;
    public CardType cardType= CardType.Zinzai;

    public int hp;
    public int skill;
    public int cost;
    public Sprite icon;
    public Ability ability;
    public Spell spell;

    //案件用
    public int time;
    public int getMoney;
    public int completeMoney;

    //インシデント用
    public IncidentType incident;
    public int IncidentPower;

    //プレビュー用
    public string flavor_text;
}

public enum Ability
{
    NONE,
    INIT_ATTACKABLE,
    SHILED,
}

public enum Spell
{
    NONE,
    DAMAGE_ZINZAI_CARD,
    DAMAGE_ZINZAI_CARDS,
    DAMAGE_ALL_ZINZAI_CARDS,
    DAMAGE_ANKEN_CARD,
    DAMAGE_ALL_ANKEN_CARD,
    DAMAGE_HERO_Money,
    DAMAGE_HERO_HP,
    HEAL_FREIND_CARD,
    HEAL_FREIND_CARDS,
    HEAL_FREIND_HERO,
}

public enum IncidentType
{
    NONE,
    hp,
    skill,
    cost,
    time,
    getMoney,
    completeMoney
}

public enum CardType
{
    NONE,
    Anken,
    Zinzai,
    Kanri,
    Incident,
    Minus,
    Taisyoku,
}