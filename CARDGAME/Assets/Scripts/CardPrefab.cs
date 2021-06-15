using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPrefab : MonoBehaviour
{
    //人材用
    public CardController ZinzaiCardPrefab;
    public GameObject ZinzaiBoardCreaturePrefab;
    public GameObject ZinzaiCardViewPrefab;

    //案件用
    public CardController AnkenCardPrefab;
    public GameObject AnkenBoardCreaturePrefab;
    public GameObject AnkenCardViewPrefab;

    //管理者用
    public CardController KanriCardPrefab;
    public GameObject KanriCardViewPrefab;

    //インシデント
    public CardController IncidentCardPrefab;
    public GameObject IncidentCardViewPrefab;

    //マイナス
    public CardController MinusCardPrefab;
    public GameObject MinusBoardCreaturePrefab;
    public GameObject MinusCardViewPrefab;


    public CardController GetCardPrefab(CardType cardType)
    {
        CardController cardController = null;
        switch (cardType)
        {
            case CardType.NONE:
                
                break;
            case CardType.Anken:
                cardController = AnkenCardPrefab;
                break;
            case CardType.Zinzai:
                cardController = ZinzaiCardPrefab;
                break;
            case CardType.Kanri:
                cardController = KanriCardPrefab;
                break;
            case CardType.Incident:
                cardController = IncidentCardPrefab;
                break;
            case CardType.Minus:
                cardController = MinusCardPrefab;
                break;
        }
        return cardController;
    }

    public GameObject GetBoardCreaturePrefab(CardType cardType)
    {
        GameObject BoardCreature = null;
        switch (cardType)
        {
            case CardType.NONE:

                break;
            case CardType.Anken:
                BoardCreature = AnkenBoardCreaturePrefab;
                break;
            case CardType.Zinzai:
                BoardCreature = ZinzaiBoardCreaturePrefab;
                break;
            case CardType.Kanri:
             //   BoardCreature = KanriCardPrefab;
                break;
            case CardType.Incident:
             //   BoardCreature = IncidentCardPrefab;
                break;
            case CardType.Minus:
                BoardCreature = MinusBoardCreaturePrefab;
                break;
        }
        return BoardCreature;
    }

    public GameObject GetCardViewPrefab(CardType cardType)
    {
        GameObject CardViewPrefab = null;
        switch (cardType)
        {
            case CardType.NONE:
                CardViewPrefab = AnkenCardViewPrefab;
                break;
            case CardType.Anken:
                CardViewPrefab = AnkenCardViewPrefab;
                break;
            case CardType.Zinzai:
                CardViewPrefab = ZinzaiCardViewPrefab;
                break;
            case CardType.Kanri:
                CardViewPrefab = KanriCardViewPrefab;
                break;
            case CardType.Incident:
                CardViewPrefab = IncidentCardViewPrefab;
                break;
            case CardType.Minus:
                CardViewPrefab = MinusCardViewPrefab;
                break;
        }
        return CardViewPrefab;
    }
}
