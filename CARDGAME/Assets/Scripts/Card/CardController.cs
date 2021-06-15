using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{

    CardView _view;//(見かけに関する操作)
    public CardModel _model; //データに関する操作
    public CardMovement _movement; //移動に関する操作

    GameManager gameManager;

    public int id
    {
        get { return _model.id; }
    }

    public bool IsSpell
    {
        get { return _model.spell != Spell.NONE; }
    }

    public CardType cardType
    {
        get { return _model.cardType; }
    }

    public bool IsHighlighted
    {
        get { return _model.IsHighlighted; }
        set { _model.IsHighlighted=IsHighlighted; }
    }


    public bool IsFieldCard
    {
        get { return _model.isFieldCard; }
        set { _model.isFieldCard = IsFieldCard; }
    }


    public bool isPreview
    {
        get { return _model.isPreview; }
        set { _model.isPreview = isPreview; }
    }

    private void Awake()
    {
        _model = GetComponent<CardModel>();
        _view = GetComponent<CardView>();
        _movement = GetComponent<CardMovement>();
        gameManager = GameManager.instace;
    }


    public void Init(int CardId, bool isPlayer,CardType cardType)
    {
        // _model = new CardModel(CardId, isPlayer, cardType);
        _model.SetCardModel(CardId, isPlayer, cardType);
        SetCard();
    }

    public void ReInit(CardModel cardModel)
    {
        _model.SetCardModel(cardModel);
        SetCard();
    }


    public void Attack(CardController enemyCard)
    {
        _model.Attack(enemyCard);
        SetCanAttack(false);
    }

    public void SetCanAttack(bool canAttack)
    {
        _model.canAttack = canAttack;
        _view.SetActiveSelectablePanel(canAttack);
    }

    public void CheckAlive()
    {
        if (_model.isAlive)
        {
            RefreshView();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void Onfield()
    {
        gameManager.ReduceManaCost(_model.cost);
        _model.isFieldCard = true;
        if (_model.ability == Ability.INIT_ATTACKABLE)
        {
            SetCanAttack(true);
        }
    }

    public void Heal(CardController freindCard)
    {
        _model.Heal(freindCard);
        freindCard.RefreshView();
    }

    public void RefreshView()
    {
        _view.Refresh(_model);

        //死亡判定
        if (_model.hp<=0)
        {
            Destroy(this.gameObject);
        }
    }

    public void Show()
    {
        _model.isMask = false;
        _view.Show(_model);
    }

    public void SetCard()
    {
        _view.SetCard(_model);
        _view.IsHighlighted(_model.IsHighlighted);
    }

    /*敵がいないのに攻撃しようとしている
     * 
     */
    /*
    public void UseSpellTo(CardController targetCard)
    {
        switch(_model.spell)
        {
            case Spell.DAMAGE_ENEMY_CARD:
                //特定の敵を攻撃する
                if (targetCard==null)
                {
                    return;
                }
                if (targetCard._model.isPlayerCard == _model.isPlayerCard)
                {
                    return;
                }
                Attack(targetCard);
                targetCard.CheckAlive();
                break;
            case Spell.DAMAGE_ENEMY_CARDS:
                //相手フィールドカードをすべて取得
                CardController[] enemyCards = gameManager.GetEnemyFieldCards(this._model.isPlayerCard);
                foreach (CardController enemyCard in enemyCards)
                {
                    Attack(enemyCard);
                }
                foreach (CardController enemyCard in enemyCards)
                {
                    enemyCard.CheckAlive();
                }
                break;
            case Spell.DAMAGE_ENEMY_HERO:
                gameManager.AttackToHero(this);
                break;
            case Spell.HEAL_FREIND_CARD:
                if (targetCard == null)
                {
                    return;
                }
                if (targetCard._model.isPlayerCard != _model.isPlayerCard)
                {
                    return;
                }
                Heal(targetCard);
                break;
            case Spell.HEAL_FREIND_CARDS:
                //相手フィールドカードをすべて取得
                CardController[] fiendCards = gameManager.GetFriendFieldCards(this._model.isPlayerCard);
                foreach (CardController firensCard in fiendCards)
                {
                    Heal(firensCard);
                }
                break;
            case Spell.HEAL_FREIND_HERO:
                gameManager.HealToHero(this);
                break;
            case Spell.NONE:
                return;
                break;
        }
        gameManager.ReduceManaCost(_model.cost, _model.isPlayerCard);
        Destroy(this.gameObject);
    }
    */

    //インシデントカードによる被害
    public void EffectedIncidentCard(CardController targetCard)
    {
        switch (targetCard._model.incident)
        {
            case IncidentType.NONE:
                Debug.LogError("NONE！");
                break;
            case IncidentType.hp:
                this._model.hp -= targetCard._model.IncidentPower;
                break;
            case IncidentType.skill:
                this._model.skill -= targetCard._model.IncidentPower;
                break;
            case IncidentType.cost:
                this._model.cost += targetCard._model.IncidentPower;
                break;
            case IncidentType.time:
                this._model.time += targetCard._model.IncidentPower;
                break;
            case IncidentType.getMoney:
                this._model.getMoney -= targetCard._model.IncidentPower;
                break;
            case IncidentType.completeMoney:
                this._model.completeMoney -= targetCard._model.IncidentPower;
                break;
        }
        this.RefreshView();        
    }

        /*
        public bool CanUseSpell()
        {
            switch (_model.spell)
            {
                case Spell.DAMAGE_ENEMY_CARD:
                case Spell.DAMAGE_ENEMY_CARDS:
                    CardController[] enemyCards = gameManager.GetEnemyFieldCards(this._model.isPlayerCard);
                    if (enemyCards.Length>0)
                    {
                        return true;
                    }
                    return false;
                case Spell.DAMAGE_ENEMY_HERO:
                    return true;
                case Spell.HEAL_FREIND_CARD:
                case Spell.HEAL_FREIND_CARDS:
                    //相手フィールドカードをすべて取得
                    CardController[] fiendCards = gameManager.GetFriendFieldCards(this._model.isPlayerCard);
                    if (fiendCards.Length > 0)
                    {
                        return true;
                    }
                    return false;
                case Spell.HEAL_FREIND_HERO:
                    return true;
                case Spell.NONE:
                    return false;
            }
            return false;
        }
        */

        public void SetHighlightingEnabled(bool enabled)
    {
        _model.IsHighlighted = enabled;
        _view.IsHighlighted(_model.IsHighlighted);
    }

    public virtual void PopulateWithInfo(int CardId,CardType cardtype)
    {
        Init(CardId, true, cardtype);
        _view.previewShow(_model);
    }

    //案件タイプ専用
    public void ClearOneTerm()
    {
        if (cardType != CardType.Anken) return;
        _model.time--;
    }

    public bool ChkComplete()
    {
        if (cardType != CardType.Anken)
        {
            Debug.LogError("不正なタイプです！");
            return false;
        }
        if (_model.time <= 0) return true;
        return false;
    }


}

