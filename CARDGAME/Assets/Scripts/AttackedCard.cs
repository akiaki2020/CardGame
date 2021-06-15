using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class AttackedCard : MonoBehaviour,IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        //攻撃
        //フィールドのカードリストを取得
        //attackカードの選択
        CardController attacker = eventData.pointerDrag.GetComponent<CardController>();

        //defenderカードを選択(Playerfeildから選択)
        CardController defender = GetComponent<CardController>();
        if (attacker==null || defender==null)
        {
            return;
        }
        if (attacker._model.isPlayerCard == defender._model.isPlayerCard)
        {
            return;
        }

        //敵フィールドにシールドカードがあり、シールドカード以外は攻撃できない
        CardController[] enemyfieldcardList = GameManager.instace.GetEnemyFieldCards(attacker._model.isPlayerCard);
        while (Array.Exists(enemyfieldcardList, card => card._model.ability == Ability.SHILED)
            && defender._model.ability!=Ability.SHILED)
        {
            return;
        }

        if (attacker._model.canAttack)
        {
            //attackとdefenderを戦わせる
            GameManager.instace.CardsBattle(attacker, defender);
        }
    }

}
