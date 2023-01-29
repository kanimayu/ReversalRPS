using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    List<Card> cardList = new List<Card>();
    [SerializeField]List<GameObject> trashCardObjectList;
    public void Start()
    {
        this.gameObject.SetActive(false);
        Clear();
    }
    public void Add(Card card)
    {
        cardList.Add(card);
        ResetTrashView();
    }
    public void Clear()
    {
        for (int i = 0; i < trashCardObjectList.Count; i++)
        {
            GameObject cardBack = trashCardObjectList[i].transform.Find("CardBack").gameObject;
            cardBack.SetActive(true);
        }
        cardList.Clear();
    }

    public void ResetTrashView()
    {
        cardList.Sort((card0, card1) => card0.Base.Number - card1.Base.Number);
        for (int i = 0; i < cardList.Count; i++)
        {
            for (int n = 0; n < trashCardObjectList.Count; n++)
            {
                if(trashCardObjectList[n].name == cardList[i].getName)
                {
                    GameObject cardBack = trashCardObjectList[n].transform.Find("CardBack").gameObject;
                    cardBack.SetActive(false);
                }
            }
        }
    }
}
