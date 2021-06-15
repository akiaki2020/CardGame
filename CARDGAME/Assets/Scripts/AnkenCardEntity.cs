using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Anken", menuName = "Create AnkenCard Entity")]
//カードデータそのもの
public class AnkenCardEntity : CardEntity
{

    public int time;
    public int GetMoney;
    public int ComleteMoney;

    private  AnkenCardEntity()
    {
        cardType = CardType.Anken;
    }

}

