using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameUI : MonoBehaviour
{
    [SerializeField] Text turnResultText;
    [SerializeField] Text playerLifeText;
    [SerializeField] Text enemyLifeText;
    [SerializeField] GameObject resultPanel;
    [SerializeField] Text resultText;
    [SerializeField] GameObject rulePanel;
    [SerializeField] GameObject leavePanel;
    [SerializeField] GameObject retryMessage;
    [SerializeField] GameObject ResultRetryButton;
    [SerializeField] GameObject ResultRetryMatchingAnim;



    public void Init()
    {
        turnResultText.gameObject.SetActive(false);
        resultPanel.SetActive(false);
        rulePanel.SetActive(false);
        leavePanel.SetActive(false);
        retryMessage.SetActive(false);
        ResultRetryMatchingAnim.SetActive(false);
    }
    public void ShowLifes(int playerLife,int enemyLife)
    {
        playerLifeText.text = $"x{playerLife}";
        enemyLifeText.text = $"x{enemyLife}";
    }

    public void ShowTurnResult(string result)
    {
        turnResultText.gameObject.SetActive(true);
        turnResultText.text = result;
    }
    public void ShowGameResult(string result)
    {
        resultPanel.SetActive(true);
        resultText.text = result;
    }

    public void SetupNextTurn()
    {
        turnResultText.gameObject.SetActive(false);
    }

    public void ShowRuleResult()
    {
        rulePanel.transform.localScale = Vector3.zero;
        rulePanel.SetActive(true);
        rulePanel.transform.DOScale(1, 0.3f);
    }
    public void CloseRuleResult()
    {
        rulePanel.transform.DOScale(0, 0.3f);
    }
    public void ShowLeaveResult()
    {
        leavePanel.transform.localScale = Vector3.zero;
        leavePanel.SetActive(true);
        leavePanel.transform.DOScale(1, 0.3f);
    }
    public void ShowRetryMessage()
    {
        retryMessage.transform.localScale = Vector3.zero;
        retryMessage.SetActive(true);
        retryMessage.transform.DOScale(1, 0.1f);
    }
    public void HideRetryButton()
    {
        ResultRetryButton.SetActive(false);
        retryMessage.SetActive(false);
        ResultRetryMatchingAnim.SetActive(true);
    }
}
