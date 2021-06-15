using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using TMPro;


public class GameManager : MonoBehaviour
{
    //シングルトン化(どこからでもアクセスできるようにする)
    public static GameManager instace;

    public enum Phase
    {
        INIT,
        ZINZAI_ANKEN,
        INCIDENT,
        SETTLEMENT,
        PHASE_MAX
    }

    public Phase _phase;
    public int turn;
    [SerializeField]
     GameObject _turnBotton;
    [SerializeField]
    private GameObject _firstPanel;

    [SerializeField]
    private GameObject _tipsPanel, _currentTip;
                        

    public GamePlayerManager player, enemy,_anken,_zinzai;
    [SerializeField] AI _enemyAi;
    [SerializeField] Settlement _SettlementAi;
    [SerializeField] UIManager _uIManager;

    [SerializeField]
    CardPrefab _cardPrefab;

    [SerializeField]
    TipsController _tipsPrefab;
    /*
    [SerializeField]
    private GameObject spellCardViewPrefab;
    [SerializeField]
    private GameObject boardCreaturePrefab;
    [SerializeField]
    private GameObject creatureCardViewPrefab;
    */
    [SerializeField]
    private GameObject fightTargetingArrowPrefab;
    [SerializeField]
    private GameObject opponentTargetingArrowPrefab;

    [SerializeField]
    public Transform _playerHandTransform,
                        _enemyHandTransform,
                        _enemyFieldTransform,
                        _playerFieldTransform,
                        _zinzaiHandTransform,
                        _ankenHandTransform;


    //public bool _isPlayerTurn;
    public GameObject _playerHero;

    //手札
    public Dictionary<string, List<CardController>> _handCardsList = new Dictionary<string, List<CardController>>();
    //protected List<CardController> _handCardsList = new List<CardController>();

    //複数のフィールドを想定
    public List<FieldController> _fieldLists;
    //各フィールドのカードを格納
    //public Dictionary<string,Cardfield.CardFieldData> _fieldsCards = new Dictionary<string, Cardfield.CardFieldData>();

    protected CardController currentSpellCard;

    protected List<GameObject> opponentHandCards = new List<GameObject>();
    /*
    protected List<BoardCreature> playerBoardCards = new List<BoardCreature>();
    protected List<BoardCreature> opponentBoardCards = new List<BoardCreature>();
    protected List<BoardCreature> playerGraveyardCards = new List<BoardCreature>();
    protected List<BoardCreature> opponentGraveyardCards = new List<BoardCreature>();

    protected BoardCreature currentCreature;
    */
    //カード選択およびマウスが重なりあった時の処理
    public bool isCardSelected;
    protected bool isPreviewActive;
    protected int currentPreviewedCardId;
    protected GameObject currentCardPreview;
    protected Coroutine createPreviewCoroutine;


    //時間管理
    int _timeCount;
    const int _defaultTime = 20;



    // Start is called before the first frame update
    void Start()
    {
        if (instace == null)
        {
            instace = this;
        }
        _phase = Phase.INIT;
        StartGame();
    }

    void StartGame()
    {
        if (_phase != Phase.INIT) ErrorLog();
        _turnBotton.SetActive(false);
        _uIManager.HideResultPanel();
        player.Init(new List<int>() { 1,2,3, 1, 2, 3, 1});
        enemy.Init(new List<int>() { 1, 2, 3, 1, 2, 3, 1 });
        _anken.Init(new List<int>() { 6,7,8,9,10, 6, 7, 8, 9, 10 });
        _zinzai.Init(new List<int>() { 100, 101, 102, 103, 104, 100, 101, 102, 103, 104 });


        _uIManager.ShowHeroHp(player.heroHP, enemy.heroHP);
        _uIManager.ShowManaCost(player.heroMoney, enemy.heroMoney);
        //SettingInitHand();
        RefreshField();

        turn = 0;
        _uIManager.ShowTurn(turn);

        //手札情報設定
        _handCardsList.Add(_playerHandTransform.name, new List<CardController>());
        _handCardsList.Add(_zinzaiHandTransform.name, new List<CardController>());
        _handCardsList.Add(_ankenHandTransform.name, new List<CardController>());
        _handCardsList.Add(_enemyHandTransform.name, new List<CardController>());

        RearrangeHand();
        _currentTip = null;
        _firstPanel.SetActive(true);
        //ChangeTurn();
    }

    private void ErrorLog()
    {
        Debug.LogError("Phaseエラー！(_phase : " + _phase + ")");
    }

    public void RefreshField()
    {
        foreach (var field in _fieldLists)
        {
            if (field.GetType() == CardType.Zinzai
                || field.GetType() == CardType.Anken)
            {
                RearrangeBottomBoard(field.gameObject);
                field.Refresh();
            }
        }
    }


    void Update()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0)&& _phase == Phase.ZINZAI_ANKEN)
        {

            var hits = Physics2D.RaycastAll(mousePos, Vector2.zero);
            var hitCards = new List<GameObject>();
            foreach (var hit in hits)
            {
                if (hit.collider != null &&
                    hit.collider.gameObject != null &&
                    hit.collider.gameObject.GetComponent<CardController>() != null &&
                    hit.collider.gameObject.GetComponent<CardController>()._model.isPlayerCard)
                {
                    hitCards.Add(hit.collider.gameObject);
                }
            }
            if (hitCards.Count > 0)
            {
                hitCards = hitCards.OrderBy(x => x.GetComponent<SortingGroup>().sortingOrder).ToList();
                var topmostCardView = hitCards[hitCards.Count - 1].GetComponent<CardController>();
                var topmostHandCard = topmostCardView.GetComponent<CardMovement>();

                //人材のときは費用がかかる
                if (topmostCardView._model.cardType == CardType.Zinzai)
                {
                    //手札上のカードの時
                    if (topmostHandCard != null &&
                        player.heroMoney >= topmostCardView._model.cost
                        && !topmostCardView._model.isFieldCard)
                    {
                        topmostCardView.GetComponent<CardMovement>().OnSelected();
                    }

                    //フィールドカード
                    if (topmostHandCard != null
                        && topmostCardView._model.isFieldCard)
                    {
                        topmostCardView.GetComponent<CardMovement>().OnSelected();
                    }
                }
                //
                else
                {   //フィールドカード
                    if (topmostHandCard != null)
                    {
                        topmostCardView.GetComponent<CardMovement>().OnSelected();
                    }
                }
            }
        }
        //ドラッグされてないとき
        else if (!isCardSelected)
        {
            var hits = Physics2D.RaycastAll(mousePos, Vector2.zero);
            var hitCards = new List<GameObject>();
            var hitHandCard = false;
            var hitBoardCard = false;
            //手札に触れたとき
            foreach (var hit in hits)
            {
                if (hit.collider != null &&
                    hit.collider.gameObject != null &&
                    hit.collider.gameObject.GetComponent<CardController>() != null)
                {
                    hitCards.Add(hit.collider.gameObject);
                    hitHandCard = true;
                }
            }
            if (hitHandCard)
            {
                if (hitCards.Count > 0)
                {
                    hitCards = hitCards.OrderBy(x => x.GetComponent<SortingGroup>().sortingOrder).ToList();
                    CardController topmostCardView = hitCards[hitCards.Count - 1].GetComponent<CardController>();

                    if (!topmostCardView.isPreview)
                    {
                        if (!isPreviewActive || topmostCardView.gameObject.GetInstanceID() != currentPreviewedCardId)
                        {
                            DestroyCardPreview();
                            CreateCardPreview(topmostCardView, topmostCardView.transform.position,
                                true, topmostCardView.IsHighlighted);
                        }
                    }
                }
            }
            else
            {
                DestroyCardPreview();
            }
        }
        else
        {
            DestroyCardPreview();
        }
        
    }

    public virtual void CreateCardPreview(CardController card, Vector3 pos, bool isHand
        ,bool highlight = false)
    {
        isPreviewActive = true;
        currentPreviewedCardId = card.gameObject.GetInstanceID();
        createPreviewCoroutine = StartCoroutine(CreateCardPreviewAsync(card, pos, highlight, isHand));
    }

    protected virtual IEnumerator CreateCardPreviewAsync(CardController card, Vector3 pos, bool highlight, bool isHand)
    {
        yield return new WaitForSeconds(0.3f);
        card.isPreview = true;

        //var gameConfig = GameManager.Instance.config;
        CardType cardType = card.cardType;
        /*
        if (cardType == "Creature")
        {
            currentCardPreview = Instantiate(creatureCardViewPrefab as GameObject);
        }
        else if (cardType.name == "Spell")
        {
            currentCardPreview = Instantiate(spellCardViewPrefab as GameObject);
        }
        */
        currentCardPreview = Instantiate(_cardPrefab.GetCardViewPrefab(card.cardType) as GameObject);
        var cardView = currentCardPreview.GetComponent<CardController>();
        cardView.PopulateWithInfo(card.id,card.cardType);
        cardView.ReInit(card._model);
        cardView.SetHighlightingEnabled(highlight);

        var newPos = Vector3.zero;
        currentCardPreview.transform.position = newPos;
        currentCardPreview.transform.localRotation = Quaternion.Euler(Vector3.zero);
        currentCardPreview.transform.localScale = new Vector2(1.5f, 1.5f);
        currentCardPreview.GetComponent<SortingGroup>().sortingOrder = 1000;
        currentCardPreview.layer = LayerMask.NameToLayer("Ignore Raycast");
        currentCardPreview.transform.DOMoveY(newPos.y + 1.0f, 0.1f);
    }

    public virtual void DestroyCardPreview()
    {
        DestroyCardPreviewInternal();
        if (createPreviewCoroutine != null)
        {
            StopCoroutine(createPreviewCoroutine);
            createPreviewCoroutine = null;
        }
        isPreviewActive = false;
    }

    protected virtual void DestroyCardPreviewInternal()
    {
        if (currentCardPreview != null)
        {
            var oldCardPreview = currentCardPreview;
            currentCardPreview = null;
            foreach (var renderer in oldCardPreview.GetComponentsInChildren<SpriteRenderer>())
            {
                renderer.DOFade(0.0f, 0.2f);
            }
            foreach (var text in oldCardPreview.GetComponentsInChildren<TextMeshPro>())
            {
                text.DOFade(0.0f, 0.2f);
            }

            var seq = DOTween.Sequence();
            seq.AppendInterval(0.5f);
            seq.AppendCallback(() =>
            {
                if (oldCardPreview.gameObject != null)
                {
                    Destroy(oldCardPreview.gameObject);
                    oldCardPreview = null;
                }
            });
        }
    }


    //ドラッグカードをドロップする
    public void PlayCard(CardController card,GameObject board)
    {
        if (card._model.isPlayerCard)
        {
            var spell = card._model.spell;
            //List<CardController> boardCardList = GetFieldCards(board);

            //スペルじゃないければその場に残す
            if (spell == Spell.NONE)
            {
                if (!card.IsFieldCard)
                {
                    var boardCreature = Instantiate(_cardPrefab.GetBoardCreaturePrefab(card.cardType));
                    CardController boardCreatureCard = boardCreature.GetComponent<CardController>();

                    boardCreatureCard.ReInit(card._model);
                    boardCreatureCard.SetCard();
                    boardCreatureCard.gameObject.tag = "PlayerOwned";
                    boardCreatureCard.transform.parent = board.transform;
                    //boardCreature.transform.position = new Vector2(1.9f * boardCardList.Count, 0);
                    boardCreatureCard.transform.position = card.transform.position;

                    //ボード特有の初期化
                    boardCreatureCard._model.isPlayerCard = true;
                    boardCreatureCard._model.isFieldCard = true;
                    boardCreatureCard.SetHighlightingEnabled(false);


                    //ドラッグしたカードを削除
                    Destroy(card.gameObject);

                    //人材ならプレイヤーの資金を減らす
                    if (boardCreatureCard._model.cardType == CardType.Zinzai)
                    {
                        ReduceManaCost(boardCreature.GetComponent<CardController>()._model.cost);
                    }

                    RearrangeBottomBoard(board, () =>
                    {
                        //フィールド情報再作成
                        RefreshField();
                        //手札並び直し
                        RearrangeAllHand();
                        //ハイライトセット
                        SetHandCardsHightLight();

                    });
                }

                //場に出した人材を移動させたとき
                else
                {
                    var boardCreature = Instantiate(_cardPrefab.GetBoardCreaturePrefab(card.cardType));
                    CardController boardCreatureCard = boardCreature.GetComponent<CardController>();
                    boardCreatureCard.ReInit(card._model);
                    boardCreatureCard.SetCard();
                    boardCreatureCard.gameObject.tag = "PlayerOwned";
                    boardCreatureCard.transform.parent = board.transform;
                    boardCreatureCard.transform.position = card.transform.position;

                    //ボード特有の初期化
                    boardCreatureCard._model.isPlayerCard = true;
                    boardCreatureCard._model.isFieldCard = true;
                    boardCreatureCard.SetHighlightingEnabled(false);
                    //ドラッグしたカードを削除
                    Destroy(card.gameObject);

                    RearrangeBottomBoard(board, () =>
                    {
                        //フィールド情報再作成
                        RefreshField();
                    });

                }

            }
            /*
            else if (cardType.name == "Spell")
            {
                var spellsPivot = GameObject.Find("PlayerSpellsPivot");
                var sequence = DOTween.Sequence();
                sequence.Append(card.transform.DOMove(spellsPivot.transform.position, 0.5f));
                sequence.Insert(0, card.transform.DORotate(Vector3.zero, 0.2f));
                sequence.Play().OnComplete(() =>
                {
                    card.GetComponent<SortingGroup>().sortingLayerName = "BoardCards";
                    card.GetComponent<SortingGroup>().sortingOrder = 1000;

                    var boardSpell = card.gameObject.AddComponent<BoardSpell>();

                    var triggeredAbilities = libraryCard.abilities.FindAll(x => x is TriggeredAbility);
                    TriggeredAbility targetableAbility = null;
                    foreach (var ability in triggeredAbilities)
                    {
                        var triggeredAbility = ability as TriggeredAbility;
                        var trigger = triggeredAbility.trigger as OnCardEnteredZoneTrigger;
                        if (trigger != null && trigger.zoneId == boardZone.zoneId && triggeredAbility.target is IUserTarget)
                        {
                            targetableAbility = triggeredAbility;
                            break;
                        }
                    }

                    currentSpellCard = card;

                    if (targetableAbility != null && effectSolver.AreTargetsAvailable(targetableAbility.effect, card.card, targetableAbility.target))
                    {
                        var targetingArrow = Instantiate(spellTargetingArrowPrefab).GetComponent<SpellTargetingArrow>();
                        boardSpell.targetingArrow = targetingArrow;
                        targetingArrow.effectTarget = targetableAbility.target;
                        targetingArrow.targetType = targetableAbility.target.GetTarget();
                        targetingArrow.onTargetSelected += () =>
                        {
                            base.PlayCard(card.card, targetingArrow.targetInfo);
                            effectSolver.MoveCard(netId, card.card, "Hand", "Board", targetingArrow.targetInfo);
                            currentSpellCard = null;
                            gameUI.endTurnButton.SetEnabled(true);
                        };
                        targetingArrow.Begin(boardSpell.transform.localPosition);
                    }
                    else
                    {
                        base.PlayCard(card.card);
                        effectSolver.MoveCard(netId, card.card, "Hand", "Board");
                        currentSpellCard = null;
                        gameUI.endTurnButton.SetEnabled(true);
                    }
                });
            }
            */
        }
        else
        {
            card.GetComponent<CardMovement>().ResetToInitialPosition();
        }
    }


    public void EnemyPlayCard(CardController card, GameObject board)
    {
        if (!card._model.isPlayerCard)
        {
            var spell = card._model.spell;
            //List<CardController> boardCardList = GetFieldCards(board);

            //スペルじゃないければその場に残す

            card.tag = "OpponentOwned";
            card.transform.parent = board.transform;
            card.Show();
            //card.transform.position = new Vector2(1.9f * boardCardList.Count, 0);
            //card.transform.position = new Vector2(1.9f * boardCardList.Count, 0);
            //card.GetComponent<CardController>()._model.isPlayerCard = true;
            //boardCreature.GetComponent<CardController>().PopulateWithInfo(card.card);

            //出したHANDの手札を捨てる
            /*
            List<CardController> handCardsList
            = GetHandCards(card.gameObject.transform.parent.gameObject);
            handCardsList.Remove(card);
            */

            //フィールドの整備
            //boardCardList.Add(card.GetComponent<CardController>());
            RearrangeBottomBoard(board, null);
    
        }
        else
        {
            card.GetComponent<CardMovement>().ResetToInitialPosition();
        }
    }


    //改修対象
    public void ReduceManaCost(int cost)
    {
        player.heroMoney -= cost;
        StartCoroutine(_uIManager.PopText(false,-cost));
        _uIManager.ShowManaCost(player.heroMoney, enemy.heroMoney);
    }

    //改修対象
    public void ReduceHeroHp(int Hp )
    {

        player.heroHP -= Hp;
        StartCoroutine(_uIManager.PopText(true, -Hp));
        _uIManager.ShowHeroHp(player.heroHP, enemy.heroMoney);
    }

    public void Restart()
    {
        //HandとFieldのデータを削除
        foreach (Transform card in _playerHandTransform)
        {
            Destroy(card.gameObject);
        }

        foreach (Transform card in _enemyHandTransform)
        {
            Destroy(card.gameObject);
        }

        foreach (Transform card in _playerFieldTransform)
        {
            Destroy(card.gameObject);
        }

        foreach (Transform card in _enemyFieldTransform)
        {
            Destroy(card.gameObject);
        }

        //デッキの生成
        player.deck = new List<int>() { 1, 1, 2, 2 };
        enemy.deck = new List<int>() { 1, 2, 3, 4 };

        _phase = Phase.INIT;
        StartGame();
    }

    //手札の初期化
    void SettingInitHand()
    {
        //カードをそれぞれ３枚渡す
        for (int i = 0; i < 3; i++)
        {
            //GiveCardToHand(player.deck, _playerHandTransform,CardType.Kanri);
            //GiveCardToHand(enemy.deck, _enemyHandTransform, CardType.Incident);
            GiveCardToHand(UnityEngine.Random.Range(1,11), _enemyHandTransform, CardType.Incident);
        }
    }



    //案件手札の初期化
    void SettingInitZin_AnHand()
    {
        for (int i = 0; i < 5; i++)
        {
            //GiveCardToHand(_anken.deck, _ankenHandTransform,CardType.Anken);
            //GiveCardToHand(_zinzai.deck, _zinzaiHandTransform, CardType.Zinzai);
            GiveCardToHand(UnityEngine.Random.Range(1, 10), _ankenHandTransform,CardType.Anken);
            GiveCardToHand(UnityEngine.Random.Range(1, 11), _zinzaiHandTransform, CardType.Zinzai);

        }
    }


    void GiveCardToHand(List<int> deck, Transform hand, CardType cardType)
    {
        if (deck.Count == 0)
        {
            return;
        }
        int cardID = deck[0];
        deck.RemoveAt(0);
        CreateCard(cardID, hand, cardType);
    }

    void GiveCardToHand(int cardId, Transform hand, CardType cardType)
    {

        int cardID = cardId;
        CreateCard(cardID, hand, cardType);
    }

    void CreateCard(int cardID, Transform hand,CardType cardType)
    {
        CardController card = Instantiate(_cardPrefab.GetCardPrefab(cardType), hand, false);

        switch (cardType)
        {
            case CardType.NONE:
                break;
            case CardType.Anken:
                card.Init(cardID, true, cardType);
                break;
            case CardType.Zinzai:
                card.Init(cardID, true, cardType);
                break;
            case CardType.Kanri:
                card.Init(cardID, true, cardType);
                break;
            case CardType.Incident:
                card.Init(cardID, false, cardType);
                break;
            case CardType.Minus:
                card.Init(cardID, false, cardType);
                break;
        }
    }


    void CreateTips()
    {
        if (_currentTip!=null)
        {
            Destroy(_currentTip);
        }
        TipsController tip = Instantiate(_tipsPrefab, _tipsPanel.transform);

        switch (_phase)
        {
            case Phase.INIT:
                break;
            case Phase.ZINZAI_ANKEN:
                tip.Init(UnityEngine.Random.Range(1,6), _phase);
                break;
            case Phase.INCIDENT:
                tip.Init(1, _phase);
                break;
            case Phase.SETTLEMENT:
                tip.Init(UnityEngine.Random.Range(1, 5), _phase);
                break;
            case Phase.PHASE_MAX:
                break;
        }
        _currentTip = tip.gameObject;

    }

    void TurnCalc()
    {
        Debug.Log("ターン:" + _phase);
        StopAllCoroutines();
        CreateTips();
        //StartCoroutine(CountDown()); //デバッグ作業のとき煩わしいので
        switch (_phase)
        {
            case Phase.INIT:
                Debug.LogError("INITターンはない！");
                break;
            case Phase.ZINZAI_ANKEN:
                _turnBotton.SetActive(true);
                Zin_AnTurn();
                break;
            case Phase.INCIDENT:
                SettingInitHand();
                StartCoroutine(_enemyAi.EnemyTurn());
                break;
            case Phase.SETTLEMENT:
                StartCoroutine(_SettlementAi.SettlementTurn());
                break;
            case Phase.PHASE_MAX:
                break;
        }

    }

    //8秒たったらターンを変える
    IEnumerator CountDown()
    {
        _timeCount = _defaultTime;
        _uIManager.UpdateTime(_timeCount);
        while (_timeCount > 0)
        {
            yield return new WaitForSeconds(1);//1秒待機
            _timeCount--;
            _uIManager.UpdateTime(_timeCount);
        }
        ChangeTurn();
    }

    //敵から見た敵→プレイヤー
    public CardController[] GetEnemyFieldCards(bool isPlayer)
    {
        if (isPlayer)
        {
            return _enemyFieldTransform.GetComponentsInChildren<CardController>();
        }
        else
        {
            return _playerFieldTransform.GetComponentsInChildren<CardController>();
        }
    }

    //
    public CardController[] GetFriendFieldCards(bool isPlayer)
    {
        if (isPlayer)
        {
            return _playerFieldTransform.GetComponentsInChildren<CardController>();
        }
        else
        {
            return _enemyFieldTransform.GetComponentsInChildren<CardController>();
        }
    }

    public void OnClickTurnEndButton()
    {
        if (_phase == Phase.INIT)
        {
            _firstPanel.SetActive(false);
            ChangeTurn();
            return;
        }

        if (_phase==Phase.ZINZAI_ANKEN)
        {
            _turnBotton.SetActive(false);
            ChangeTurn();
            return;
        }
    }
    
    public void ChangeTurn()
    {
        

        //フィールドカードを攻撃不可能にする
        CardController[] playefieldcardList = _playerFieldTransform.GetComponentsInChildren<CardController>();
        SettingCardAttackView(playefieldcardList, false);
        CardController[] enemyfieldcardList = _enemyFieldTransform.GetComponentsInChildren<CardController>();
        SettingCardAttackView(enemyfieldcardList, false);

        _phase++;
        if (_phase == Phase.PHASE_MAX)
        {
            turn++;
            _uIManager.ShowTurn(turn);
            _phase = Phase.ZINZAI_ANKEN;
        }

        if (_phase - 1 == Phase.ZINZAI_ANKEN)
        {
            DigardCard(_zinzaiHandTransform.gameObject);
            DigardCard(_ankenHandTransform.gameObject, 
                () => StartCoroutine
                    (_uIManager.ShowPhaseText
                        (
                            ()=>TurnCalc()
                        )
                    )
                );

        }
        else
        {
            StartCoroutine
                   (_uIManager.ShowPhaseText
                       (
                           () => TurnCalc()
                       )
                   );
        }

    }

    public void SettingCardAttackView(CardController[] fieldcardList,bool canAttack)
    {
        foreach (CardController card in fieldcardList)
        {
            //カードを攻撃可能にする
            card.SetCanAttack(canAttack);
        }
    }
 
    void PlayerTurn()
    {
        //フィールドカードを攻撃可能にする
        CardController[] playefieldcardList = _playerFieldTransform.GetComponentsInChildren<CardController>();
        SettingCardAttackView(playefieldcardList, true);
        RearrangeAllHand();
    }

    void Zin_AnTurn()
    {
        SettingInitZin_AnHand();
        RearrangeAllHand();
        //ハイライトセット
        SetHandCardsHightLight();
    }

    public void CardsBattle(CardController attacker, CardController defender)
    {
        Debug.Log("CardsBattle");
 
        attacker.Attack(defender);
        defender.Attack(attacker);

        attacker.CheckAlive();
        defender.CheckAlive();
    }

    public void AttackToHero(CardController attacker)
    {
        if (attacker._model.isPlayerCard)
        {
            enemy.heroHP -= attacker._model.skill;
        }
        else
        {
            player.heroHP -= attacker._model.skill;
        }
        _uIManager.ShowHeroHp(player.heroHP, enemy.heroHP);

        attacker.SetCanAttack(false);
    }

    public void HealToHero(CardController healer)
    {
        if (healer._model.isPlayerCard)
        {
            player.heroHP += healer._model.skill;
        }
        else
        {
            enemy.heroHP += healer._model.skill;
        }
        _uIManager.ShowHeroHp(player.heroHP, enemy.heroHP);
    }

    public void CheckHeroHp()
    {
        if (player.heroHP<=0||enemy.heroHP<=0)
        {
            _uIManager.ShowResultPanel(player.heroHP);
        }
    }

    public void ReshowHeroState()
    {
        _uIManager.ShowHeroHp(player.heroHP, enemy.heroHP);
        _uIManager.ShowManaCost(player.heroMoney, enemy.heroMoney);
        CheckHeroHp();
    }

    public void SettlementHeroStateCalc()
    {
        ReduceManaCost(1);
        if (player.heroMoney <= 0 )
        {
            ReduceHeroHp(1);
        }
        ReshowHeroState();
    }

    public void SetHandCardsHightLight()
    {
        SetHandCardHightLight(_zinzaiHandTransform.gameObject);
        SetHandCardHightLight(_ankenHandTransform.gameObject);
    }

    //引数にDESTROYを入れること
    public void SetHandCardHightLight(GameObject hand, Action onComplete = null)
    {
        List<CardController> playerHandCards = new List<CardController>();
        CardController[] handCards = hand.GetComponentsInChildren<CardController>();
        playerHandCards.AddRange(handCards);
        for (var i = 0; i < playerHandCards.Count; i++)
        {
            var card = playerHandCards[i];

            if (card._model.cardType == CardType.Zinzai)
            {
                if (player.heroMoney >= card._model.cost)
                {
                    card.SetHighlightingEnabled(true);
                }
                else
                {
                    card.SetHighlightingEnabled(false);
                }
            }
            else
            {
                card.SetHighlightingEnabled(true);
            }

        }
        if (onComplete != null)
        {
            onComplete();
        }
    }


    protected virtual void RearrangeAllHand()
    {
        RearrangeHand();
        RearrangeZin_AnHand(_zinzaiHandTransform);
        RearrangeZin_AnHand(_ankenHandTransform);
    }

    protected virtual void RearrangeZin_AnHand(Transform field)
    {
        //出したHANDの手札を手札に入れる

        var handWidth = 0.0f;
        var spacing = 2f;
        List<CardController> playerHandCards = new List<CardController>();
        CardController[] handCards = field.GetComponentsInChildren<CardController>();
        playerHandCards.AddRange(handCards);
       // playerHandCards = GetHandCards(field.gameObject);

        foreach (var card in handCards)
        {
            handWidth += card.GetComponent<SpriteRenderer>().bounds.size.y;
            handWidth += spacing;
            //MASKも外す
            card.Show();
        }
        handWidth -= spacing;
        var pivot = field.position;
        for (var i = 0; i < playerHandCards.Count; i++)
        {
            var card = playerHandCards[i];
            card.transform.position = new Vector3(pivot.x, pivot.y - handWidth / 2, 0.0f);
            pivot.y += handWidth / playerHandCards.Count;
            card.GetComponent<SortingGroup>().sortingLayerName = "HandCards";
            card.GetComponent<SortingGroup>().sortingOrder = playerHandCards.Count - i;
        }

        

    }

    //管理者手札の整理
    protected virtual void RearrangeHand()
    {
        
        var handWidth = 0.0f;
        var spacing = 2.2f;
        List<CardController> playerHandCards = new List<CardController>();
        CardController[] handCards = _playerHandTransform.GetComponentsInChildren<CardController>();
        playerHandCards.AddRange(handCards);
        //playerHandCards        = GetHandCards(_playerHandTransform.gameObject);

        foreach (var card in handCards)
        {
            handWidth += card.GetComponent<SpriteRenderer>().bounds.size.x;
            handWidth += spacing;
        }
        handWidth -= spacing;

        var pivot = _playerHandTransform.position;
        var totalTwist = -20f;
        if (playerHandCards.Count == 1)
        {
            totalTwist = 0;
        }
        var twistPerCard = totalTwist / playerHandCards.Count;
        float startTwist = -1f * (totalTwist / 2);
        var scalingFactor = 0.06f;
        for (var i = 0; i < playerHandCards.Count; i++)
        {
            var card = playerHandCards[i];
            var twist = startTwist + (i * twistPerCard);
            var nudge = Mathf.Abs(twist);
            nudge *= scalingFactor;
            card.transform.position = new Vector3(pivot.x - handWidth / 2, pivot.y - nudge,0.0f);
            card.transform.rotation = Quaternion.Euler(0, 0, twist);
            pivot.x += handWidth / playerHandCards.Count;
            card.GetComponent<SortingGroup>().sortingLayerName = "HandCards";
            card.GetComponent<SortingGroup>().sortingOrder = playerHandCards.Count-i;
        }
    }

    //終了後引数の関数を実行する
    //フィールドの再配置を行う
    protected virtual void RearrangeBottomBoard(GameObject board, Action onComplete = null)
    {
        
        var boardWidth = 0.0f;
        var spacing = -0.3f;
        var cardWidth = 0.0f;
        List<CardController> BoardCards = new List<CardController>();
        CardController[] cards = board.GetComponentsInChildren<CardController>();
        BoardCards.AddRange(cards);
        //BoardCards=GetFieldCards(board);

        foreach (var card in BoardCards)
        {
            cardWidth = card.GetComponent<BoxCollider2D>().bounds.size.x;
            boardWidth += cardWidth+ spacing;
        }
        boardWidth -= spacing;
        var newPositions = new List<Vector2>(BoardCards.Count);
        var pivot = board.transform.position;
        for (var i = 0; i < BoardCards.Count; i++)
        {
            var card = BoardCards[i];
            newPositions.Add(new Vector2(pivot.x - boardWidth / 2 + cardWidth / 2, pivot.y));
            pivot.x += boardWidth / BoardCards.Count;
        }
         var sequence = DOTween.Sequence();
        for (var i = 0; i < BoardCards.Count; i++)
        {
            var card = BoardCards[i];
            sequence.Insert(0, card.transform.DOMove(newPositions[i], 0.4f).SetEase(Ease.OutSine));
        }
        sequence.OnComplete(() =>
        {
            if (onComplete != null)
            {
                onComplete();
            }
        });
    }

    public List<CardController> GetHandCards(GameObject hand)
    {
        List<CardController> handCards = null;
        _handCardsList.TryGetValue(hand.name, out handCards);
        return handCards;
    }

    public List<CardController> GetFieldCards(GameObject board)
    {
        FieldController filedController;
        List<CardController> boardCards = null;
        filedController = _fieldLists.Find(n => n.gameObject.GetInstanceID() == board.GetInstanceID());

        boardCards = filedController.GetCardList();
        return boardCards;
    }

    public CardType GetFieldCardType(GameObject board)
    {
        FieldController filedController;
        filedController = _fieldLists.Find(n => n.gameObject.GetInstanceID() == board.GetInstanceID());
        CardType fieldType= filedController.GetType();
        return fieldType;
    }

    //引数にDESTROYを入れること
    public void DigardCard(GameObject hand, Action onComplete = null)
    {
        List<CardController> playerHandCards = new List<CardController>();
        CardController[] handCards = hand.GetComponentsInChildren<CardController>();
        playerHandCards.AddRange(handCards);
        var newPositions = new Vector2(hand.transform.position.x, 10);
        var sequence = DOTween.Sequence();
        for (var i = 0; i < playerHandCards.Count; i++)
        {
            var card = playerHandCards[i];
            sequence.Insert(0, card.transform.DOMove(newPositions, 0.4f).SetEase(Ease.OutSine));
        }
        sequence.OnComplete(() =>
        {
            for (var i = 0; i < playerHandCards.Count; i++)
            {
                var card = playerHandCards[i];
                Destroy(card.gameObject);
            }
            if (onComplete != null)
            {
                onComplete();
            }
        });
    }
}
