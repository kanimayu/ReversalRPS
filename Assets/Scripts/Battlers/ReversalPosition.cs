using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ReversalPosition : MonoBehaviour
{
    Card reversalCard;

    public Card ReversalCard { get => reversalCard; }

    public void Set(Card card)
    {
        reversalCard = card;
        card.transform.SetParent(transform);
        card.transform.DOMove(transform.position, 0.1f);
        card.transform.DORotate(new Vector3(0,0,90), 0.1f);
    }

    public void DeleteCard()
    {
        reversalCard = null;
    }
}
