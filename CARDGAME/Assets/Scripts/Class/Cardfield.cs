using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cardfield
{
    public struct CardFieldData
    {
        public CardType m_type;
        public List<CardController> m_cardList;

        public CardFieldData(CardType type)
        {
            m_type = type;
            m_cardList = new List<CardController>();
        }
    }
}
