using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settlement : MonoBehaviour
{
    [SerializeField]
    GameManager _gameManager = null;


    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameManager.instace;
    }

    /*
     * 構造　この関数が呼ばれた時点ですでにハンドに敵フィールドに配られている
     */
    public IEnumerator SettlementTurn()
    {
        if (_gameManager == null)
        {
            _gameManager = GameManager.instace;
        }
        //フィールドを取得
        List<FieldController> _fieldLists = _gameManager._fieldLists;
        FieldController[] fields = _fieldLists.ToArray();

        //案件フィールドのみ絞って計算を行う
        FieldController[] ankenFields = Array.FindAll(fields, field => (field.GetType() == CardType.Anken));
        foreach (var field in ankenFields)
        {
            if (field.GetComponentsInChildren<CardController>().Length <= 0) continue;
            //自分の人材フィールドを取得
            if (field._zinzaiField == null) Debug.LogError("人材フィールドなし！");
            FieldController zinzaifeild = field._zinzaiField;
            //全員のスキルが上回っていた場合
            if (zinzaifeild.GetSkill >= field.GetSkill)
            {
                //炎上Effectを消す
                Debug.Log("field.setBurning" + false);

                field.setBurning(false);

                //プレイヤーの資金を増やす
                _gameManager.ReduceManaCost(-field.GetMoney);
                yield return new WaitForSeconds(1);


                //案件カードの総工数を減らす
                CardController[] AnkenCardList = field.GetComponentsInChildren<CardController>();
                CardController ankkenCard = AnkenCardList[0];
                ankkenCard.ClearOneTerm();

                //総工程が終わっていた場合
                if (ankkenCard.ChkComplete())
                {
                    //プレイヤーの資金を増やす
                    _gameManager.ReduceManaCost(-field._model.completeMoney);
                    //案件フィールドにあったカードを全部消す
                    foreach (var card in AnkenCardList)
                    {
                        Destroy(card.gameObject);
                        field.ReInit();
                    }

                    yield return new WaitForSeconds(1);
                }
             }
            //下回った場合炎上Effectを出す
            else
            {   
                field.setBurning(true);
            }
        }
        yield return new WaitForSeconds(1);

        foreach (var field in ankenFields)
        {
            //自分の人材フィールドを取得
            if (field._zinzaiField == null) Debug.LogError("人材フィールドなし！");
            FieldController zinzaifeild = field._zinzaiField;
            //雇っている分金額を減らす
            int zinzaiCost = 0;
            CardController[] zinzaiLists = zinzaifeild.GetComponentsInChildren<CardController>();
            foreach (var zinzai in zinzaiLists)
            {
                zinzaiCost += zinzai._model.cost;
            }
            //プレイヤーの資金を減らす
            if(zinzaiCost!=0)_gameManager.ReduceManaCost(zinzaiCost);
        }
        yield return new WaitForSeconds(1);
        _gameManager.RefreshField();
        _gameManager.SettlementHeroStateCalc();
        _gameManager.ChangeTurn();

    }
}
