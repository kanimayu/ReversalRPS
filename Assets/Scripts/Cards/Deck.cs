using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class Deck : MonoBehaviourPunCallbacks
{
    List<Card> list = new List<Card>();
    List<int> shuffleNumList = new List<int>(13);

    public void Start()
    {
        for (int i = 0; i < 13; i++)
        {
            shuffleNumList.Add(i);
        }
        ShuffleNumList();
    }

    public void ShuffleNumList()
    {
        for (int i = 0; i < shuffleNumList.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, shuffleNumList.Count);
            int temp = shuffleNumList[i];
            shuffleNumList[i] = shuffleNumList[randomIndex];
            shuffleNumList[randomIndex] = temp;
        }
    }

    public void SyncShuffleNumList()
    {
        List<int> syncNumList = new List<int>(13);
        for (int i = 0; i < 13; i++)
        {
            syncNumList.Add((PhotonNetwork.CurrentRoom.CustomProperties[i.ToString()] is int value) ? value : 0);
        }
        shuffleNumList = syncNumList;
    }

    public void Add(Card card)
    {
        list.Add(card);
        card.transform.SetParent(transform);
    }
    public void Remove(Card card)
    {
        list.Remove(card);
    }

    public void ResetPosition()
    {
        list.Sort((card0, card1) => card0.Base.Number - card1.Base.Number);
        for (int i = 0; i < list.Count; i++)
        {
            float posX = 0;
            list[i].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            list[i].transform.localPosition = new Vector3(posX, 0);
        }
    }

    public Card RandomRemove()
    {
        int r = shuffleNumList[0];
        Card card = list.Find(x => x.Base.Number == r);
        shuffleNumList.RemoveAt(0);
        Remove(card);
        return card;
    }

    public Card Remove(int number)
    {
        Card card = list.Find(x => x.Base.Number == number);
        Remove(card);
        return card;
    }

}
