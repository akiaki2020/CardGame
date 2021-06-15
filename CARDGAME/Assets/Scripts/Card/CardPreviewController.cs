using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPreviewController : CardController
{
    //カード情報表示
    public  void PopulateWithInfo()
    {
        SetCard();
        RefreshView();
    }

}

