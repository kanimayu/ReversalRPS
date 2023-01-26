using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    List<Card> list = new List<Card>();

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
            float posX = (i - list.Count / 10f + 0.5f);
            list[i].transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
            list[i].transform.localPosition = new Vector3(posX, 0);
        }
    }

    public Card RandomRemove()
    {
        int r = Random.Range(0, list.Count);
        Card card = list[r];
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
