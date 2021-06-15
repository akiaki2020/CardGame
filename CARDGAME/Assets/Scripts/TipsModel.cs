using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//カードデータとその処理
public class TipsModel : MonoBehaviour
{

    //プレビュー用
    public string text;

    public void SetTipsModel(int cardId, GameManager.Phase phase)
    {
        TipsEntity cardEntity = null;
        switch (phase)
        {
            case GameManager.Phase.INIT:
                cardEntity = Resources.Load<TipsEntity>("TipsEntityList/Tips " + cardId);
                break;
            case GameManager.Phase.ZINZAI_ANKEN:
                cardEntity = Resources.Load<TipsEntity>("TipsEntityList/ZINZAI_ANKEN/Tips " + cardId);
                break;
            case GameManager.Phase.INCIDENT:
                cardEntity = Resources.Load<TipsEntity>("TipsEntityList/INCIDENT/Tips " + cardId);
                break;
            case GameManager.Phase.SETTLEMENT:
                cardEntity = Resources.Load<TipsEntity>("TipsEntityList/SETTLEMENT/Tips " + cardId);
                break;
            case GameManager.Phase.PHASE_MAX:
                break;
        }

        //プレビュー用
        text = cardEntity.text;
    }
}
