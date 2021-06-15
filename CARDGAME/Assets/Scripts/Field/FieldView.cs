using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class FieldView : MonoBehaviour
{
    [SerializeField] TextMeshPro nameText;
    [SerializeField] TextMeshPro timeText;
    [SerializeField] TextMeshPro skillText;
    [SerializeField] TextMeshPro costText;
    [SerializeField] TextMeshPro getMoneyText;
    [SerializeField] TextMeshPro completeText;
    [SerializeField] GameObject BurningEffect;

    public void InitField(FieldModel model)
    {
        switch (model.m_type)
        {
            case CardType.NONE:
                break;
            case CardType.Anken:
                nameText.text = model.ankenName;
                timeText.text = model.time.ToString();
                skillText.text = model.skill.ToString();
                getMoneyText.text = model.getMoney.ToString();
                completeText.text = model.completeMoney.ToString();
                break;
            case CardType.Zinzai:
                skillText.text = model.skill.ToString();
                costText.text = model.cost.ToString();
                break;
            case CardType.Kanri:
                break;
            case CardType.Incident:
                break;
            case CardType.Minus:
                break;
        }
    }

    public void Refresh(FieldModel model)
    {
        switch (model.m_type)
        {
            case CardType.NONE:
                break;
            case CardType.Anken:
                nameText.text = model.ankenName;
                timeText.text = model.time.ToString();
                skillText.text = model.skill.ToString();
                getMoneyText.text = model.getMoney.ToString();
                completeText.text = model.completeMoney.ToString();
                break;
            case CardType.Zinzai:
                skillText.text = model.skill.ToString();
                costText.text = model.cost.ToString();
                break;
            case CardType.Kanri:
                break;
            case CardType.Incident:
                break;
            case CardType.Minus:
                break;
        }
    }

    public void Burning(Boolean burnflg)
    {
        BurningEffect.SetActive(burnflg);
    }
}

