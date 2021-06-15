using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//カードデータとその処理
public class CardModel : MonoBehaviour
{
    public int id;
    public string name;
    public int hp;
    public int skill;
    public int cost;
    public Sprite icon;
    public Ability ability;
    public Spell spell;
    public CardType cardType;

    //案件用
    public int time;
    public int getMoney;
    public int completeMoney;

    //プレビュー用
    public string flavor_text;

    //インシデント用
    public IncidentType incident;
    public int IncidentPower;
    public bool isMask=false;


    public bool isAlive;
    public bool canAttack;
    public bool isFieldCard;
    public bool isPlayerCard;
    public bool isPreview;
    public bool IsHighlighted;




    public CardModel(int cardId,bool isPlayer,CardType cardType)
    {
        CardEntity cardEntity=null;
        switch (cardType)
        {
            case CardType.NONE:
                cardEntity = Resources.Load<CardEntity>("CardEntityList/Card " + cardId);
                break;
            case CardType.Anken:
                cardEntity = Resources.Load<CardEntity>("CardEntityList/Anken/Card " + cardId);
                break;
            case CardType.Zinzai:
                cardEntity = Resources.Load<CardEntity>("CardEntityList/Zinzai/Card " + cardId);
                break;
            case CardType.Kanri:
                cardEntity = Resources.Load<CardEntity>("CardEntityList/Kanri/Card " + cardId);
                break;
            case CardType.Incident:
                cardEntity = Resources.Load<CardEntity>("CardEntityList/Incident/Card " + cardId);
                isMask = true;
                break;
            case CardType.Minus:
                cardEntity = Resources.Load<CardEntity>("CardEntityList/Minus/Card " + cardId);
                break;
        }
        //Debug.Log("id: "+ cardId);
        //Debug.Log("cardType: " + cardType);
        //Debug.Log("name: " + cardEntity.name);
        id = cardId;
        name = cardEntity.name;
        hp = cardEntity.hp;
        skill = cardEntity.skill;
        cost = cardEntity.cost;
        icon = cardEntity.icon;
        ability = cardEntity.ability;
        spell= cardEntity.spell;
        cardType = cardEntity.cardType;
        Debug.Log("cardType: " + cardType);

        //案件用
        time = cardEntity.time;
        getMoney = cardEntity.getMoney;
        completeMoney = cardEntity.completeMoney;

        //インシデント用
        incident = cardEntity.incident;
        IncidentPower= cardEntity.IncidentPower;

        //プレビュー用
        flavor_text = cardEntity.flavor_text;
        isAlive = true;
        isFieldCard = false;
        isPlayerCard = isPlayer;
        isPreview= false;
        IsHighlighted = false;
    }

    public void SetCardModel(int cardId, bool isPlayer, CardType Type)
    {
        CardEntity cardEntity = null;
        switch (Type)
        {
            case CardType.NONE:
                cardEntity = Resources.Load<CardEntity>("CardEntityList/Card " + cardId);
                break;
            case CardType.Anken:
                cardEntity = Resources.Load<CardEntity>("CardEntityList/Anken/Card " + cardId);
                break;
            case CardType.Zinzai:
                cardEntity = Resources.Load<CardEntity>("CardEntityList/Zinzai/Card " + cardId);
                break;
            case CardType.Kanri:
                cardEntity = Resources.Load<CardEntity>("CardEntityList/Kanri/Card " + cardId);
                break;
            case CardType.Incident:
                cardEntity = Resources.Load<CardEntity>("CardEntityList/Incident/Card " + cardId);
                isMask = true;
                break;
            case CardType.Minus:
                cardEntity = Resources.Load<CardEntity>("CardEntityList/Minus/Card " + cardId);
                break;
        }
        id = cardId;
        name = cardEntity.name;
        hp = cardEntity.hp;
        skill = cardEntity.skill;
        cost = cardEntity.cost;
        icon = cardEntity.icon;
        ability = cardEntity.ability;
        spell = cardEntity.spell;
        cardType = Type;

        //案件用
        time = cardEntity.time;
        getMoney = cardEntity.getMoney;
        completeMoney = cardEntity.completeMoney;

        //インシデント用
        incident = cardEntity.incident;
        IncidentPower = cardEntity.IncidentPower;

        //プレビュー用
        flavor_text = cardEntity.flavor_text;
        isAlive = true;
        isFieldCard = false;
        isPlayerCard = isPlayer;
        isPreview = false;
        IsHighlighted = false;
    }


    // MemberwiseCloneメソッドを使用
    public void SetCardModel(CardModel motoCard)
    {
        id = motoCard.id;
        name = motoCard.name;
        hp = motoCard.hp;
        skill = motoCard.skill;
        cost = motoCard.cost;
        icon = motoCard.icon;
        ability = motoCard.ability;
        spell = motoCard.spell;
        cardType = motoCard.cardType;

        //案件用
        time = motoCard.time;
        getMoney = motoCard.getMoney;
        completeMoney = motoCard.completeMoney;

        //インシデント用
        incident = motoCard.incident;
        IncidentPower = motoCard.IncidentPower;

        //プレビュー用
        flavor_text = motoCard.flavor_text;
        isAlive = true;
        isFieldCard = false;
        isPlayerCard = motoCard.isPlayerCard;
        isPreview = false;
        IsHighlighted = false;
    }


    void RecoveryHP(int point)
    {
        hp += point;
    }

    void Damage(int dmg)
    {
        hp -= dmg;
        if (hp<=0)
        {
            hp = 0;
            isAlive = false ;
        }
    }

    public void Attack(CardController card)
    {
        card._model.Damage(skill);
    }

    //Cardを回復させる
    public void Heal(CardController card)
    {
        card._model.RecoveryHP(skill);
    }
}
