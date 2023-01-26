using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using KanKikuchi.AudioManager;

public class OnlineMenuManager : MonoBehaviourPunCallbacks
{
    // ボタンを押したらマッチング開始
    // ランダムマッチングで誰かの部屋に入れればマッチング成功
    // 部屋がなければ自分で作る
    // 部屋が2名になればシーンを遷移
    [SerializeField] GameObject loadingAnim;
    [SerializeField] GameObject matchingButton;
    [SerializeField] GameObject matchingMessage;
    [SerializeField] GameObject cancelButton;
    [SerializeField] GameObject cpuButton;
    bool inRoom;
    bool isMatched;

    public void OnMatchingButton()
    {
        loadingAnim.SetActive(true);
        matchingButton.SetActive(false);
        matchingMessage.SetActive(true);
        cpuButton.SetActive(false);
        StartCoroutine(ShowCancelButton());
        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();
    }

    IEnumerator ShowCancelButton()
    {
        yield return new WaitForSeconds(0.8f);
        cancelButton.SetActive(true);
    }

    public void OnCancelButton()
    {
        loadingAnim.SetActive(false);
        matchingButton.SetActive(true);
        matchingMessage.SetActive(false);
        cpuButton.SetActive(true);
        cancelButton.SetActive(false);
        // 接続を切る
        inRoom = false;
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
    }

    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    // ゲームサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        inRoom = true;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 2}, TypedLobby.Default);
    }

    // 部屋が2人ならシーンを変える
    private void Update()
    {
        if (isMatched)
        {
            return;
        }
        if (inRoom)
        {
            if (PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                isMatched = true;
                BGMManager.Instance.FadeOut();
                SceneManager.LoadScene("Game");
            }
        }
    }
}
