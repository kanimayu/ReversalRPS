using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using KanKikuchi.AudioManager;

public class GameMaster : MonoBehaviourPunCallbacks
{
    [SerializeField] Battler player;
    [SerializeField] Battler enemy;
    [SerializeField] CardGenerator cardGenerator;
    [SerializeField] GameUI gameUI;
    [SerializeField] Deck deck;
    [SerializeField] Trash trash;

    RuleBook rulebook;
    bool playerRetryReady;
    bool enemyRetryReady;
    bool fieldIsReversed;


    // Start is called before the first frame update
    private void Awake()
    {
        rulebook = GetComponent<RuleBook>();
    }


    // Start is called before the first frame update
    private void Start()
    {
        Setup();
    }

    void Setup()
    {
        BGMManager.Instance.Play(BGMPath.BGM_BATTLEFIELD_F2);
        gameUI.Init();
        player.Life = 5;
        enemy.Life = 5;
        playerRetryReady = false;
        enemyRetryReady = false;
        fieldIsReversed = false;
        gameUI.ShowLifes(player.Life, enemy.Life);
        player.OnSubmitAction = SubmittedAction;
        enemy.OnSubmitAction = SubmittedAction;
        SendCardsToDeck(deck);
        SendCardsTo(player, isEnemy: false, deck);
        SendCardsTo(enemy, isEnemy: true, deck);
        deck.ResetPosition();
        OpenFinalCard(deck);
        player.Hand.ResetPosition();
        if (GameDataManager.Instance.IsOnlineBattle == true)
        {
            player.OnSubmitAction += SendPlayerCard;
        }
    }

    void SubmittedAction()
    {
        if (player.IsSubmitted && enemy.IsSubmitted)
        {
            StartCoroutine(CardsBattle());
        }
        else if (player.IsSubmitted)
        {
            if (GameDataManager.Instance.IsOnlineBattle == false)
            {
                enemy.RandomSubmit();
            }

        }
        else if (enemy.IsSubmitted)
        {

        }
    }

    void SendCardsToDeck(Deck deck)
    {
        for (int i = 0; i < 13; i++)
        {
            Card card = cardGenerator.Spawn(i, true);
            deck.Add(card);
        }
    }

    void SendCardsTo(Battler battler, bool isEnemy, Deck deck)
    {
        for (int i = 0; i < 5; i++)
        {
            Card card = deck.RandomRemove();
            battler.SetCardToHand(card);
            if (isEnemy)
            {
                //card.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
            else
            {
                card.Open();
            }
        }
        battler.Hand.ResetPosition();
    }

    void OpenFinalCard(Deck deck)
    {
        Card card = deck.RandomRemove();
        card.Open();
        trash.Add(card);
    }

    // Update is called once per frame
    IEnumerator CardsBattle()
    {
        yield return new WaitForSeconds(0.8f);
        enemy.SubmitCard.Open();
        yield return new WaitForSeconds(0.8f);
        // 場を逆転にする
        if (fieldIsReversed)
        {
            if (player.IsReversed && enemy.IsReversed)
            {
                // 逆転返し
                gameUI.ShowTurnResult("逆転返し");
                gameUI.ReverseBG(true);
                yield return new WaitForSeconds(1.5f);
                fieldIsReversed = false;
            }
        }
        else
        {
            if (player.IsReversed && enemy.IsReversed)
            {
                // 一発逆転返し
                gameUI.ShowTurnResult("逆転");
                gameUI.ReverseBG(false);
                yield return new WaitForSeconds(1.5f);
                gameUI.ShowTurnResult("逆転返し");
                gameUI.ReverseBG(true);
                yield return new WaitForSeconds(1.5f);
                fieldIsReversed = false;
            }
            else if (player.IsReversed || enemy.IsReversed)
            {
                // 逆転
                gameUI.ShowTurnResult("逆転");
                gameUI.ReverseBG(false);
                yield return new WaitForSeconds(1.5f);
                fieldIsReversed = true;
            }

        }

        Result result = rulebook.GetResult(player, enemy, fieldIsReversed);
        switch (result)
        {
            case Result.TurnWin:
                gameUI.ShowTurnResult("WIN");
                enemy.Life--;
                break;
            case Result.TurnLose:
                gameUI.ShowTurnResult("LOSE");
                player.Life--;
                break;
            case Result.TurnDraw:
                gameUI.ShowTurnResult("DRAW");
                break;

        }
        yield return new WaitForSeconds(1.5f);
        gameUI.ShowLifes(player.Life, enemy.Life);

        if (player.Life <= 0 || enemy.Life <= 0 || result == Result.GameWin || result == Result.GameLose)
        {
            ShowResult(result);
        }
        else
        {
            SetupNextTurn();
        }

    }

    void ShowResult(Result result)
    {
        if (enemy.Life <= 0 || result == Result.GameWin)
        {
            gameUI.ShowGameResult("WIN");
        }
        else if (player.Life <= 0 || result == Result.GameLose)
        {
            gameUI.ShowGameResult("LOSE");
        }
        else
        {
            gameUI.ShowGameResult("DRAW");
        }
    }

    void SetupNextTurn()
    {
        trash.Add(player.SubmitCard);
        trash.Add(enemy.SubmitCard);
        player.SetupNextTurn();
        enemy.SetupNextTurn();
        gameUI.SetupNextTurn();
    }

    void SendPlayerCard()
    {
        photonView.RPC(nameof(RPCOnRecievedCard), RpcTarget.Others, player.SubmitCard.Base.Number);
    }

    [PunRPC]
    void RPCOnRecievedCard(int number)
    {
        enemy.SetSubmitCard(number);
    }

    public void OnRetryButton()
    {
        playerRetryReady = true;
        gameUI.HideRetryButton();
        if (GameDataManager.Instance.IsOnlineBattle)
        {
            SendRetryMessage();
            if (playerRetryReady && enemyRetryReady)
            {
                FadeManager.Instance.LoadScene("Game", 1f);
            }
        }
        else
        {
            FadeManager.Instance.LoadScene("Game", 1f);
        }
    }
    public void OnTitleButton()
    {
        if (GameDataManager.Instance.IsOnlineBattle)
        {
            gameUI.ShowLeaveResult();
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.Disconnect();
            }
        }
        FadeManager.Instance.LoadScene("Title", 1f);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (GameDataManager.Instance.IsOnlineBattle)
        {
            gameUI.ShowLeaveResult();
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.Disconnect();
            }
        }
    }

    void SendRetryMessage()
    {
        photonView.RPC(nameof(RPCOnRecieveRetryMessage), RpcTarget.Others);
    }

    [PunRPC]
    void RPCOnRecieveRetryMessage()
    {
        enemyRetryReady = true;
        if (playerRetryReady)
        {
            FadeManager.Instance.LoadScene("Game", 1f);
        }
        else
        {
            gameUI.ShowRetryMessage();
        }
    }
}
