using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlace : MonoBehaviour,IDropHandler
{
    public enum TYPE
    {
        HAND,
        FIELD
    }
    public TYPE type;
    public GameObject boardZone;

    private bool startedDrag;
    private Vector3 initialPos;
    private GameManager gameManager;
    protected CardController cardController;


    public void OnDrop(PointerEventData eventData)
    {
        if (type==TYPE.HAND)
        {
            return;
        }
        CardController card = eventData.pointerDrag.GetComponent<CardController>();
        if (card !=null)
        {
            if (!card._movement._isDraggable)
            {
                return;
            }

            if (card.IsSpell)
            {
                return;
            }

            card._movement._defaultParent = this.transform;
            if (card._model.isFieldCard)
            {
                return;
            }
            card.Onfield();
        }
    }
    /*
    private void awake()
    {
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
        if (gameManager._isPlayerTurn )
        {
            startedDrag = true;
            initialPos = transform.position;
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

        if (boardZone.GetComponent<BoxCollider2D>().bounds.Contains(transform.position))
        {
            gameManager.PlayCard(cardController);
            //cardController.SetHighlightingEnabled(false);
        }
        else
        {
            transform.position = initialPos;
        }
    }

    public void ResetToInitialPosition()
    {
        transform.position = initialPos;
    }
    */
}
