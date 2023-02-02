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
    // 場が逆転しているフラグ
    bool fieldIsReversed;
    // 逆転返し済フラグ
    bool fieldIsFixed;
    // バトル数
    int battleCount;
    // ラウンド数
    int roundCount;

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
        battleCount = 0;
        roundCount = 0;
        playerRetryReady = false;
        enemyRetryReady = false;
        fieldIsReversed = false;
        fieldIsFixed = false;
        gameUI.ShowLifes(player.Life, enemy.Life);
        player.OnSubmitAction = SubmittedAction;
        enemy.OnSubmitAction = SubmittedAction;
        SendCardsToDeck(deck);
        if (GameDataManager.Instance != null)
        {
            Debug.Log("オンライン？" + GameDataManager.Instance.IsOnlineBattle);
        }
        if (GameDataManager.Instance != null && GameDataManager.Instance.IsOnlineBattle == true)
        {
            // オンライン
            player.OnSubmitAction += SendPlayerCard;
            // 乱数同期
            deck.SyncShuffleNumList();
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("自身がマスタークライアントです");
                SendCardsTo(player, isEnemy: false, deck);
                SendCardsTo(enemy, isEnemy: true, deck);
                SendReversalCardTo(player, isEnemy: false, deck);
                SendReversalCardTo(enemy, isEnemy: true, deck);
            }
            else
            {
                Debug.Log("自身がサブクライアントです");
                SendCardsTo(enemy, isEnemy: true, deck);
                SendCardsTo(player, isEnemy: false, deck);
                SendReversalCardTo(enemy, isEnemy: true, deck);
                SendReversalCardTo(player, isEnemy: false, deck);
            }
        }
        else
        {
            // オフライン
            SendCardsTo(player, isEnemy: false, deck);
            SendCardsTo(enemy, isEnemy: true, deck);
            SendReversalCardTo(player, isEnemy: false, deck);
            SendReversalCardTo(enemy, isEnemy: true, deck);
        }
        deck.ResetPosition();
        OpenFinalCard(deck);
        player.Hand.ResetPosition();
    }

    void SubmittedAction()
    {
        if (player.IsSubmitted && enemy.IsSubmitted)
        {
            StartCoroutine(CardsBattle());
        }
        else if (player.IsSubmitted)
        {
            if (GameDataManager.Instance != null && GameDataManager.Instance.IsOnlineBattle == false)
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
        //手札の作成
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

    void SendReversalCardTo(Battler battler, bool isEnemy, Deck deck)
    {
        //逆転札の作成
        Card reversalCard = deck.RandomRemove();
        battler.SetCardToReversal(reversalCard);
    }


    void OpenFinalCard(Deck deck)
    {
        Card card = deck.RandomRemove();
        trash.Add(card);
        card.OpenAndTrash();
    }

    // Update is called once per frame
    IEnumerator CardsBattle()
    {
        yield return new WaitForSeconds(0.8f);
        enemy.SubmitCard.Open();
        yield return new WaitForSeconds(0.8f);
        // 場を逆転にする
        if (!fieldIsFixed)
        {
            if (fieldIsReversed)
            {
                if (player.IsReversed && enemy.IsReversed)
                {
                    if (player.ReversalCard != null)
                    {
                        trash.Add(player.ReversalCard);
                        player.ReversalCard.OpenAndTrash();
                    }
                    if (enemy.ReversalCard != null)
                    {
                        trash.Add(enemy.ReversalCard);
                        enemy.ReversalCard.OpenAndTrash();
                    }
                    SEManager.Instance.Play(SEPath.SFX_SUMMONLEGENDARY);
                    // 逆転返し
                    gameUI.ShowTurnResult("逆転返し");
                    gameUI.ReverseBG(true);
                    yield return new WaitForSeconds(1.5f);
                    fieldIsReversed = false;
                    fieldIsFixed = true;
                }
            }
            else
            {
                if (player.IsReversed && enemy.IsReversed)
                {
                    SEManager.Instance.Play(SEPath.SFX_SPELL_NATURALSELECTION);
                    // 一発逆転返し
                    gameUI.ShowTurnResult("逆転");
                    gameUI.ReverseBG(false);
                    trash.Add(player.ReversalCard);
                    player.ReversalCard.OpenAndTrash();
                    yield return new WaitForSeconds(1.5f);
                    SEManager.Instance.Play(SEPath.SFX_SUMMONLEGENDARY);
                    gameUI.ShowTurnResult("逆転返し");
                    gameUI.ReverseBG(true);
                    trash.Add(enemy.ReversalCard);
                    enemy.ReversalCard.OpenAndTrash();
                    yield return new WaitForSeconds(1.5f);
                    fieldIsReversed = false;
                    fieldIsFixed = true;
                }
                else if (player.IsReversed || enemy.IsReversed)
                {
                    SEManager.Instance.Play(SEPath.SFX_SPELL_NATURALSELECTION);
                    if (player.IsReversed)
                    {
                        trash.Add(player.ReversalCard);
                        player.ReversalCard.OpenAndTrash();
                    }
                    if (enemy.IsReversed)
                    {
                        trash.Add(enemy.ReversalCard);
                        enemy.ReversalCard.OpenAndTrash();
                    }
                    // 逆転
                    gameUI.ShowTurnResult("逆転");
                    gameUI.ReverseBG(false);
                    yield return new WaitForSeconds(1.5f);
                    fieldIsReversed = true;
                }
            }
        }

        Result result = rulebook.GetResult(player, enemy, fieldIsReversed);
        switch (result)
        {
            case Result.TurnWin:
                SEManager.Instance.Play(SEPath.SFX_F3_ANUBIS_ATTACK_IMPACT);
                gameUI.ShowTurnResult("WIN");
                enemy.Life--;
                break;
            case Result.TurnLose:
                SEManager.Instance.Play(SEPath.SFX_NEUTRAL_ARTIFACTHUNTER_ATTACK_IMPACT);
                gameUI.ShowTurnResult("LOSE");
                player.Life--;
                break;
            case Result.TurnDraw:
                gameUI.ShowTurnResult("DRAW");
                break;

        }
        yield return new WaitForSeconds(1.5f);
        battleCount++;
        gameUI.ShowLifes(player.Life, enemy.Life);

        if (player.Life <= 0 || enemy.Life <= 0)
        {
            //KO
            SetupNextTurn();
            ShowResult(result);
        }
        else if (battleCount == 5)
        {
            //NextRound
            SetupNextTurn();
            ShowResult(result);
        }
        else
        {
            SetupNextTurn();
        }

    }

    void ShowResult(Result result)
    {
        SEManager.Instance.Play(SEPath.SFX_VICTORY_CREST);
        if (enemy.Life < player.Life)
        {
            gameUI.ShowGameResult("WIN");
        }
        else if (player.Life < enemy.Life)
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
        player.SubmitCard.Trash();
        enemy.SubmitCard.Trash();
        player.SetupNextTurn();
        enemy.SetupNextTurn();
        gameUI.SetupNextTurn();
    }

    void SendPlayerCard()
    {
        photonView.RPC(nameof(RPCOnRecievedCard), RpcTarget.Others, player.SubmitCard.Base.Number, player.IsReversed);
    }

    [PunRPC]
    void RPCOnRecievedCard(int number,bool isReversed)
    {
        enemy.EnemySetSubmitCard(number, isReversed);
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
