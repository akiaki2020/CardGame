using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FieldController : MonoBehaviour
{

    FieldView _view;//(見かけに関する操作)
    public FieldModel _model; //データに関する操作
    public FieldController _zinzaiField;

    GameManager gameManager;

    public int GetSkill
    {
        get { return _model.skill; }
    }

    public int GetMoney
    {
        get { return _model.getMoney; }
    }

    private void Awake()
    {
        _view = GetComponent<FieldView>();
        _model = GetComponent<FieldModel>();
        gameManager = GameManager.instace;
        if (GetType() == CardType.Anken
            && _zinzaiField == null) Debug.LogError("AnkenFieldには人材フィールドが必須です");
    }


    public void Init(CardType type)
    {
        _model = new FieldModel(type);
        _view.InitField(_model);
    }

    public void ReInit()
    {
        _model.ReInitInt_2();
        _view.InitField(_model);
    }


    public void Refresh()
    {
        CardController[] fieldCards = GetComponentsInChildren<CardController>();
        SetField(_model, fieldCards);

         _view.Refresh(_model);
    }

    public CardType GetType()
    {
        return _model.m_type;
    }

    public List<CardController> GetCardList()
    {
        return _model.m_cardList;
    }

    public void SetField(FieldModel model, CardController[] fieldCards)
    {
        //数値データ初期化
        _model.ReInitInt_1();
        List<CardController> listCards = new List<CardController>();
        listCards.AddRange(fieldCards);

        for (var i = 0; i < listCards.Count; i++)
        {
            var card = listCards[i]._model;

            if (i == 0) model.ankenName = card.name;
            model.time += card.time;
            model.cost += card.cost;
            model.getMoney += card.getMoney;
            model.completeMoney += card.completeMoney;
            model.skill += card.skill;
        }

    }

    private void SetFieldDate(CardModel cardModel)
    {
    }

    public void setBurning(bool Burningflg)
    {
        if (GetType() == CardType.Anken)
        {
            _model.isBurning = Burningflg;
            
            _view.Burning(_model.isBurning);
        }
        else
        {
            Debug.LogError("案件フィールド以外起動しない！");
        }

    }

}

