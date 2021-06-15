using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayerManager : MonoBehaviour
{
    public List<int> deck = new List<int>();

    public int heroHP;
    public int heroMoney;
    public int defaultMoney;

    public void Init(List<int> cardDeck)
    {
        deck = cardDeck;
        heroHP = 10;
        heroMoney = defaultMoney = 10;
    }
    
}
