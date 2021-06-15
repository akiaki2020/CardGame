using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardMovement : MonoBehaviour//,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    public Transform _defaultParent;
    public bool _isDraggable;

    public enum TYPE
    {
        HAND,
        FIELD
    }
    public TYPE type;
    public GameObject boardZone;

    [SerializeField] private bool startedDrag;
    private Vector3 initialPos;
    private GameManager gameManager;
    public CardController cardController;


    public IEnumerator MoveToField(Transform field) 
    {
        //一度親をCanvasに変更する
        transform.SetParent(_defaultParent.parent);
        //DOTweenでカードをフィールドに移動
        transform.DOMove(field.position, 0.25f);
        yield return new WaitForSeconds(0.25f);
        _defaultParent = field;
        transform.SetParent(_defaultParent);
    }

    public IEnumerator MoveToTarget(Transform target)
    {
        //現在の位置と並びを取得
        Vector3 currentPosition = transform.position;
        int siblingIndex = transform.GetSiblingIndex();

        //一度親をCanvasに変更する
        transform.SetParent(_defaultParent.parent);
        //DOTweenでカードをフィールドに移動
        transform.DOMove(target.position, 0.25f);
        yield return new WaitForSeconds(0.25f);

        //戻り作業
        transform.DOMove(currentPosition, 0.25f);
        yield return new WaitForSeconds(0.25f);
        if (this != null)
        {
            transform.SetParent(_defaultParent);
            transform.SetSiblingIndex(siblingIndex);
        }
    }

    void Start()
    {
        _defaultParent = transform.parent;
        gameManager = GameManager.instace;
        boardZone = gameManager._playerFieldTransform.gameObject;
        cardController = this.GetComponent<CardController>();


    }


    private void awake()
    {
        _defaultParent = transform.parent;
        gameManager = GameManager.instace;
        boardZone = gameManager._playerFieldTransform.gameObject;
    }

    private void Update()
    {
        if (startedDrag)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var newPos = transform.position;
            newPos.z = 0;
            transform.position = newPos;
        }
    }

    public void OnSelected()
    {
        if (gameManager._phase==GameManager.Phase.ZINZAI_ANKEN)
        {
            startedDrag = true;
            initialPos = transform.position;
            _defaultParent = transform.parent;
            transform.parent= gameManager.transform;
            gameManager.isCardSelected = true;
        }
    }

    public void OnMouseUp()
    {
        if (!startedDrag)
        {
            return;
        }
        startedDrag = false;
        gameManager.isCardSelected = false;
        List<FieldController> fieldLists = gameManager._fieldLists;

        foreach (var field in fieldLists)
        {
            if (field.GetComponent<BoxCollider2D>().bounds.Contains(transform.position))
            {

                if (cardController.cardType == gameManager.GetFieldCardType(field.gameObject))
                {                    
                    gameManager.PlayCard(cardController, field.gameObject);
                    return;
                }

                if (cardController.IsFieldCard&& field.GetType()==CardType.Taisyoku)
                {
                    Destroy(this.gameObject);
                    gameManager.RefreshField();
                }
            }
        }
        transform.parent = _defaultParent;
        transform.position = initialPos;
    }

    public void ResetToInitialPosition()
    {
        transform.position = initialPos;
    }
}
