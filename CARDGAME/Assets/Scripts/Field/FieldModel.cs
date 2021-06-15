using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//カードデータとその処理
public class FieldModel :MonoBehaviour
{
    public string ankenName;
    [SerializeField] public int time;
    [SerializeField] public int cost;
    [SerializeField] public int getMoney;
    [SerializeField] public int completeMoney;
    [SerializeField] public int skill;
    public CardType m_type;
    [SerializeField] public List<CardController> m_cardList;
    [SerializeField] public bool isBurning=false;

    public FieldModel(CardType type)
    {
        ankenName = "";
        time = cost= completeMoney= skill= getMoney = 0;
        m_type = CardType.NONE;
        m_cardList = new List<CardController>();
        m_type = type;
    }

    public void SetCardModeltoThis(CardModel cardModel)
    {
        time = cardModel.time;

    }



    //キャラクターが置かれた時に走る処理
    public void ReInitInt_1()
    {
        //特定の数値データだけ初期化
        time = cost = completeMoney = skill = getMoney = 0;
    }

    //案件が取り除かれた時
    public void ReInitInt_2()
    {
        ankenName = "";
        //特定の数値データだけ初期化
        time = cost = completeMoney = skill = getMoney = 0;
    }

}
