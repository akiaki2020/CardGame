using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    GameManager _gameManager = null;

    [SerializeField] GameObject _resultPanel;
    [SerializeField] Text _resultText;

    [SerializeField]
    Text _playerheroHPText,
         _enemyHeroHpText;

    [SerializeField]
    GameObject _playerPopHPText,
     _playerPopMoneyText;


    [SerializeField]
    Text _playerHeroManaCostText,
         _enemyheroManaCostText;

    [SerializeField] Text _timeText;
    [SerializeField]
    Text _turnYearText,
     _turnQuaterText;
    [SerializeField]
    GameObject _phaseZinAn, _phaseInci, _phaseSettele;

    void Start()
    {
        _gameManager = GameManager.instace;
    }
    public void HideResultPanel()
    {
        _resultPanel.SetActive(false);

    }
    public void ShowHeroHp(int playerHeroHP, int enemyHeroHP)
    {
        _playerheroHPText.text = playerHeroHP.ToString();
        _enemyHeroHpText.text = enemyHeroHP.ToString();

    }

    public void ShowManaCost(int playerManaCost, int enemyManaCost)
    {
        _playerHeroManaCostText.text = playerManaCost.ToString();
        _enemyheroManaCostText.text = enemyManaCost.ToString();
    }

    public void UpdateTime(int timeCount)
    {
        _timeText.text = timeCount.ToString();
    }

    public void ShowResultPanel(int playerHeroHP)
    {
        StopAllCoroutines();
        _resultPanel.SetActive(true);
        if (playerHeroHP <= 0)
        {
            _resultText.text = "LOSE";
        }
        else
        {
            _resultText.text = "Win";
        }
    }

    public IEnumerator ShowPhaseText(Action onComplete = null)
    {
        yield return new WaitForSeconds(0.25f);
        Vector3 leftVec3 = new Vector3(-1200.0f, 0,0);
        Vector3 rightVec3 = new Vector3(1200.0f, 0,0);
        RectTransform rectTransform = null;
        GameObject moveObject=null;
        var sequence = DOTween.Sequence();

        switch (_gameManager._phase)
        {
            case GameManager.Phase.INIT:
                Debug.LogError("不正なフェーズ");
                break;
            case GameManager.Phase.ZINZAI_ANKEN:
                moveObject = _phaseZinAn;
                break;
            case GameManager.Phase.INCIDENT:
                moveObject = _phaseInci;
                break;
            case GameManager.Phase.SETTLEMENT:
                moveObject = _phaseSettele;
                break;
            case GameManager.Phase.PHASE_MAX:
                Debug.LogError("不正なフェーズ");
                break;
        }
        moveObject.GetComponent<RectTransform>().anchoredPosition = leftVec3;
        /*
        moveObject.GetComponent<RectTransform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main, leftVec3);
        rectTransform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, Vector3.zero);
        */
        sequence.Insert(0, moveObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0.0f,0.0f), 0.4f).SetEase(Ease.OutSine));
        sequence.Insert(2.0f, moveObject.GetComponent<RectTransform>().DOAnchorPos(rightVec3, 0.4f).SetEase(Ease.OutSine));
        sequence.OnComplete(() =>
        {
            onComplete();
        });
    }

    public IEnumerator PopText(bool HporMoney,int Size,Action onComplete = null)
    {
        yield return new WaitForSeconds(0.25f);
        Vector3 leftVec3 = new Vector3(-660.0f, 0, 0);
        Vector3 rightVec3 = new Vector3(660.0f, 0, 0);
        RectTransform rectTransform = null;
        GameObject moveObject = null;
        var sequence = DOTween.Sequence();

        if (HporMoney)
        {
            //HPの時
            moveObject=_playerPopHPText;
        }
        else
        {
            moveObject = _playerPopMoneyText;
        }
        Text text = moveObject.GetComponent<Text>();
        text.text = Size.ToString();
        sequence.Insert(0,
            DOTween.ToAlpha(() => text.color, color => text.color = color,
                    1.0f, // 目標値
                    0.5f // 所要時間
                )
            );
        sequence.Join(
            moveObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0.0f, 10.0f), 0.4f)
            .SetEase(Ease.OutSine)
            .SetRelative()
            );

        yield return new WaitForSeconds(1.0f);
        sequence.Insert(0,
            DOTween.ToAlpha(() => text.color, color => text.color = color,
                    0.0f, // 目標値
                    0.5f // 所要時間
                )
            );
        sequence.Join(
            moveObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0.0f, -1.0f), 0.4f)
            .SetEase(Ease.OutSine)
            .SetRelative()
            );

        sequence.OnComplete(() =>
        {
            if (onComplete !=null )
            {
                onComplete();
            }
        });
    }

    public void ShowTurn(int turnCount)
    {

        _turnYearText.text = (2020+ turnCount/4).ToString()+"年";

        switch (turnCount % 4)
        {
            case 0:
                _turnQuaterText.text ="第1四半期" ;
                break;
            case 1:
                _turnQuaterText.text = "第2四半期";
                break;
            case 2:
                _turnQuaterText.text = "第3四半期";
                break;
            case 3:
                _turnQuaterText.text = "第4四半期";
                break;
            default:
                _turnQuaterText.text = "エラー！";
                break;
        }

    }
}
