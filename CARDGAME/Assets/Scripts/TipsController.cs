using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TipsController : MonoBehaviour
{

    public TextMeshProUGUI _view;//(見かけに関する操作)
    public TipsModel _model; //データに関する操作
    GameManager gameManager;

    private void Awake()
    {
        _model = GetComponent<TipsModel>();
        _view = GetComponent<TextMeshProUGUI>();
        gameManager = GameManager.instace;
    }


    public void Init(int CardId, GameManager.Phase phase)
    {
        _model.SetTipsModel(CardId,  phase);
        setTips();
    }

    public void setTips()
    {
        _view.text = _model.text;
    }

}

