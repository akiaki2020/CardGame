using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class AI : MonoBehaviour
{
    [SerializeField]
    GameManager _gameManager = null;

    Coroutine _aiCoroutine =null;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameManager.instace;
    }

    /*
     * 構造　この関数が呼ばれた時点ですでにハンドに敵フィールドに配られている
     */
    public IEnumerator EnemyTurn()
    {
        if (_gameManager == null)
        {
            _gameManager = GameManager.instace;
        }
        //フィールドのカードリストを取得
        CardController[] enemyfieldcardList = _gameManager._enemyFieldTransform.GetComponentsInChildren<CardController>();
        //フィールドカードを攻撃可能にする
        //_gameManager.SettingCardAttackView(enemyfieldcardList, true);
        yield return new WaitForSeconds(1);

        //フィールド一枚ごとに効果を発生させる
        foreach (CardController card in enemyfieldcardList)
        {
            StartCoroutine(CastSpellOf(card));
            //待機カードをフィールドに移動させる
            //_gameManager.EnemyPlayCard(card, _gameManager._enemyFieldTransform.gameObject);
            yield return new WaitForSeconds(1.0f);

        }
        /*場にカードを出す*/
        //手札のカードリストを取得 子要素のコンポーネントを取得
        CardController[] handcardList = _gameManager._enemyHandTransform.GetComponentsInChildren<CardController>();
        /*場にカードを出す*/
        StartCoroutine(SetCardtoEnemyField());
        yield return new WaitForSeconds(1);
        _gameManager.ChangeTurn();
    }

    public IEnumerator SetCardtoEnemyField()
    {
        //フィールドのカードリストを取得
        CardController[] enemyfieldcardList = _gameManager._enemyFieldTransform.GetComponentsInChildren<CardController>();
        /*場にカードを出す*/
        //手札のカードリストを取得 子要素のコンポーネントを取得
        CardController[] handcardList = _gameManager._enemyHandTransform.GetComponentsInChildren<CardController>();
        foreach (CardController card in handcardList)
        {
            //待機カードをフィールドに移動させる
            _gameManager.EnemyPlayCard(card, _gameManager._enemyFieldTransform.gameObject);
            yield return new WaitForSeconds(0.25f);

        }

        yield return new WaitForSeconds(1);
    }

    /*
     * 対象としては
     * プレイヤー
     * 案件単体
     * 人材(単体+フィールドのみ、全体)
     * 次のマイナスカードを増やすに分かれる
     */
    IEnumerator CastSpellOf(CardController card)
    {
        switch (card._model.spell)
        {
            case Spell.NONE:
                break;
            case Spell.DAMAGE_ZINZAI_CARD:
            case Spell.DAMAGE_ZINZAI_CARDS:
            case Spell.DAMAGE_ALL_ZINZAI_CARDS:
                StartCoroutine(DAMAGE_ZINZAII(card));
                break;
            case Spell.DAMAGE_ANKEN_CARD:
            case Spell.DAMAGE_ALL_ANKEN_CARD:
                StartCoroutine(DAMAGE_ANKEN(card));
                break;
            case Spell.DAMAGE_HERO_Money:
            case Spell.DAMAGE_HERO_HP:
                StartCoroutine(DAMAGE_HERO(card));
                break;
            case Spell.HEAL_FREIND_CARD:
                break;
            case Spell.HEAL_FREIND_CARDS:
                break;
            case Spell.HEAL_FREIND_HERO:
                break;
        }
        //ターゲット/それぞれのフィールド/それぞれのHeroのTransformが必要
        //StartCoroutine(card._movement.MoveToField(movePosition));
        yield return new WaitForSeconds(0.25f);
    }

    IEnumerator DAMAGE_ZINZAII(CardController card)
    {
        //攻撃対象を決める
        //フィールドを取得
        List<FieldController> _fieldLists = _gameManager._fieldLists;
        FieldController[] fields = _fieldLists.ToArray();

        //人材フィールドかつ人材がいるフィールドを取得
       
        FieldController[] zinzaiFields = 
            Array.FindAll(fields, field => (field.GetType() == CardType.Zinzai)&&
                          field.GetComponentsInChildren<CardController>().Length > 0
                            );

        //攻撃可能でないならその時点で停止
        if (zinzaiFields.Length == 0)
        {
            //壊す
            Destroy(card.gameObject);
            Debug.Log("人材なし");
            yield break;
        } 

        if (card._model.spell == Spell.DAMAGE_ALL_ZINZAI_CARDS)
        {
            foreach (var field in zinzaiFields)
            {
                CardController[] zinzaiCardList = field.GetComponentsInChildren<CardController>();
                foreach (var zinzaicard in zinzaiCardList)
                {
                    //カードにマイナスエフェクト
                    zinzaicard.EffectedIncidentCard(card);
                }
                //そのまま二週目に
                continue;
            }
            //内部処理が終わったらカードを移動させる
           StartCoroutine(AiCardMove(card, Vector3.zero));
        }
        else if (card._model.spell == Spell.DAMAGE_ZINZAI_CARDS)
        {
            //どの人材フィールドか選択
            FieldController tempfieldController = zinzaiFields[UnityEngine.Random.Range(0, zinzaiFields.Length)];
            CardController[] zinzaiCardList = tempfieldController.GetComponentsInChildren<CardController>();
            foreach (var zinzaicard in zinzaiCardList)
            {
                //カードにマイナスエフェクト
                zinzaicard.EffectedIncidentCard(card);
            }
            //内部処理が終わったらカードを移動させる
            StartCoroutine(AiCardMove(card, tempfieldController.transform.position));
        }
        else
        {
            //カード単品の場合
            //どの人材フィールドか選択
            FieldController tempfieldController = zinzaiFields[UnityEngine.Random.Range(0, zinzaiFields.Length)];
            //人材全体を取得
            CardController[] zinzaiCardList = tempfieldController.GetComponentsInChildren<CardController>();
            CardController zinzaiCard= zinzaiCardList[UnityEngine.Random.Range(0, zinzaiCardList.Length)];
            //カードにマイナスエフェクト
            zinzaiCard.EffectedIncidentCard(card);
            //内部処理が終わったらカードを移動させる
           StartCoroutine(AiCardMove(card, zinzaiCard.transform.position));
        }

        _gameManager.RefreshField();
        yield return new WaitForSeconds(0.25f);
    }

    IEnumerator DAMAGE_ANKEN(CardController card)
    {
        //攻撃対象を決める
        //フィールドを取得
        List<FieldController> _fieldLists = _gameManager._fieldLists;
        FieldController[] fields = _fieldLists.ToArray();

        //人材フィールドかつ人材がいるフィールドを取得

        FieldController[] ankenFields =
            Array.FindAll(fields, field => (field.GetType() == CardType.Anken) &&
                          field.GetComponentsInChildren<CardController>().Length > 0
                            );

        //攻撃可能でないならその時点で停止
        if (ankenFields.Length == 0)
        {
            //壊す
            Destroy(card.gameObject);
            Debug.Log("人材なし");
            yield break;
        }

        if (card._model.spell == Spell.DAMAGE_ALL_ANKEN_CARD)
        {
            foreach (var field in ankenFields)
            {
                CardController[] ankenCardList = field.GetComponentsInChildren<CardController>();
                foreach (var zinzaicard in ankenCardList)
                {
                    //カードにマイナスエフェクト
                    zinzaicard.EffectedIncidentCard(card);
                }
                //そのまま二週目に
                continue;
            }
            //内部処理が終わったらカードを移動させる
            StartCoroutine(AiCardMove(card, Vector3.zero));
        }
        else
        {
            //カード単品の場合
            //どの人材フィールドか選択
            FieldController tempfieldController = ankenFields[UnityEngine.Random.Range(0, ankenFields.Length)];
            //人材全体を取得
            CardController[] ankenCardList = tempfieldController.GetComponentsInChildren<CardController>();
            CardController ankenCard = ankenCardList[UnityEngine.Random.Range(0, ankenCardList.Length)];
            //カードにマイナスエフェクト
            ankenCard.EffectedIncidentCard(card);
            //内部処理が終わったらカードを移動させる
            StartCoroutine(AiCardMove(card, ankenCard.transform.position));
        }

        _gameManager.RefreshField();
        yield return new WaitForSeconds(0.25f);
    }

    IEnumerator DAMAGE_HERO(CardController card)
    {
        //プレイヤー自身に攻撃
        if (card._model.spell == Spell.DAMAGE_HERO_Money)
        {
            _gameManager.ReduceManaCost(card._model.IncidentPower);
        }
        else
        {
            _gameManager.ReduceHeroHp(card._model.IncidentPower);
        }
        //内部処理が終わったらカードを移動させる
        StartCoroutine(AiCardMove(card, _gameManager._playerHero.transform.position));
        yield return new WaitForSeconds(0.25f);
    }


    IEnumerator AiCardMove(CardController card,Vector3 targetPosition)
    {
        Debug.Log("AiCardMove");
        var sequence = DOTween.Sequence();
        sequence.Insert(0, card.transform.DOMove(targetPosition, 0.7f).SetEase(Ease.OutSine));
        sequence.OnComplete(() =>
        {
            card.transform.position = targetPosition;
        });

        yield return new WaitForSeconds(1.0f);
        //壊す
        Destroy(card.gameObject);
    }
}
