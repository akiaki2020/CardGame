using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackedHero : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        //攻撃
        //フィールドのカードリストを取得
        //attackカードの選択
        CardController attacker = eventData.pointerDrag.GetComponent<CardController>();

        //defenderカードを選択(Playerfeildから選択)
        if (attacker == null )
        {
            return;
        }
        //敵フィールドにシールドカードがあれば攻撃できない
        CardController[] enemyfieldcardList = GameManager.instace.GetEnemyFieldCards(attacker._model.isPlayerCard);
        while (Array.Exists(enemyfieldcardList, card => card._model.ability == Ability.SHILED))
        {
            return;
        }

        if (attacker._model.canAttack)
        {
            //attackがHeroを攻撃する
            GameManager.instace.AttackToHero(attacker);
            GameManager.instace.CheckHeroHp();
        }
    }

}
