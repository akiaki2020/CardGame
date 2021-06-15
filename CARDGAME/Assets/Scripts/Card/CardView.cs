using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardView : MonoBehaviour
{
    [SerializeField] TextMeshPro nameText;
    [SerializeField] TextMeshPro hpText;
    [SerializeField] TextMeshPro skillText;
    [SerializeField] TextMeshPro costText;
    [SerializeField] TextMeshPro timeText;
    [SerializeField] TextMeshPro getMoneyText;
    [SerializeField] TextMeshPro CompleteText;
    [SerializeField] SpriteRenderer iconImage;
    [SerializeField] GameObject selectablePanel;
    [SerializeField] GameObject shieidPanel;
    //[SerializeField] GameObject maskPanel;
    [SerializeField] SpriteRenderer glowSprite;
    [SerializeField] TextMeshPro flavor_Text;


    public void SetCard(CardModel cardModel)
    {
        nameText.text = cardModel.name;
        if (cardModel.cardType==CardType.Zinzai)
        {
            hpText.text = "やる気: "+cardModel.hp.ToString();
            skillText.text = "技能: " + cardModel.skill.ToString();
            costText.text = "単価: " + cardModel.cost.ToString();
        }      
        else if (cardModel.cardType == CardType.Anken)
        {
            skillText.text = "必要技能: " + cardModel.skill.ToString();
            timeText.text = "残工数: " + cardModel.time.ToString();
            getMoneyText.text = "月予算: " + cardModel.getMoney.ToString();
            CompleteText.text = "完了報酬: " + cardModel.completeMoney.ToString();
        }
        
        iconImage.sprite = cardModel.icon;

        /*
        if (cardModel.isPlayerCard)
        {
            maskPanel.SetActive(false);
        }
        else
        {
            maskPanel.SetActive(true);
        }
        */
        if (cardModel.ability == Ability.SHILED)
        {
            shieidPanel.SetActive(true);

        }
        else
        {
            shieidPanel.SetActive(false);
        }

        if (cardModel.spell != Spell.NONE)
        {
            hpText.gameObject.SetActive(false);
        }
        else
        {
            hpText.gameObject.SetActive(true);
        }
    }

    public void Refresh(CardModel cardModel)
    {
        if (cardModel.cardType != CardType.Anken)
        {
            hpText.text = "やる気: " + cardModel.hp.ToString();
            skillText.text = "技能: " + cardModel.skill.ToString();
            costText.text = "単価: " + cardModel.cost.ToString();
        }
        else
        {
            skillText.text = "必要技能: " + cardModel.skill.ToString();
            timeText.text = "残工数: " + cardModel.time.ToString();
            getMoneyText.text = "月予算: " + cardModel.getMoney.ToString();
            CompleteText.text = "完了報酬: " + cardModel.completeMoney.ToString();
        }
    }

    public void SetActiveSelectablePanel(bool flg)
    {
        selectablePanel.SetActive(flg);
    }

    public void Show(CardModel cardModel)
    {
      //  maskPanel.SetActive(!cardModel.isMask);
    }



    public void IsHighlighted(bool enable)
    {
        glowSprite.enabled=enable;
    }

    //プレビュー用
    public void previewShow(CardModel cardModel)
    {
        flavor_Text.text = cardModel.flavor_text;
    }
}
