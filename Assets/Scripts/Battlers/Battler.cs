

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using KanKikuchi.AudioManager;
using UnityEngine.UI;

public class Battler : MonoBehaviour
{
    [SerializeField] BattlerHand hand;
    [SerializeField] SubmitPosition submitPosition;
    [SerializeField] ReversalPosition reversalPosition;
    [SerializeField] GameObject submitButton;
    [SerializeField] GameObject reverseButton;
    public bool IsSubmitted { get; private set; }
    public bool IsMulligan { get; private set; }
    public bool CanReverse { get; private set; }
    public bool IsReversed { get; private set; }
    public UnityAction OnSubmitAction;

    public BattlerHand Hand { get => hand; }
    public Card SubmitCard { get => submitPosition.SubmitCard; }
    public Card ReversalCard { get => reversalPosition.ReversalCard; }
    public int Life { get; set; }

    public void Start()
    {
        CanReverse = true;
        if (submitButton)
        {
            submitButton?.SetActive(false);
        }
    }

    public void SetCardToHand(Card card)
    {
        hand.Add(card);
        card.OnClickCard = SelectedCard;
    }

    public void SetCardToReversal(Card card)
    {
        reversalPosition.Set(card);
    }

    void SelectedCard(Card card)
    {
        if (IsSubmitted) return;
        if (IsMulligan)
        {
            // マリガン時
            hand.CheckMulligan(card);
            submitButton?.SetActive(true);
        }
        else
        {
            // 通常時
            if (submitPosition.SubmitCard)
            {
                hand.Add(submitPosition.SubmitCard);
            }
            hand.Remove(card);
            submitPosition.Set(card);
            hand.ResetPosition();
            submitButton?.SetActive(true);
        }
    }

    public void OnSubmitButton()
    {
        if (submitPosition.SubmitCard)
        {
            IsSubmitted = true;
            if (IsReversed)
            {
                CanReverse = false;
            }
            OnSubmitAction?.Invoke();
            submitButton?.SetActive(false);
            reverseButton?.SetActive(false);
        }
    }

    public void OnReverseButton()
    {
        if (IsReversed)
        {
            SEManager.Instance.Play(SEPath.SFX_ARTIFACT_EQUIP01);
            IsReversed = false;
            reverseButton?.transform.DOPause();
            reverseButton.GetComponent<Image>().color = new Color32(50, 50, 50, 255);
        }
        else
        {
            SEManager.Instance.Play(SEPath.SFX_ARTIFACT_EQUIP);
            IsReversed = true;
            reverseButton?.transform.DOPunchScale(punch: Vector3.one * 0.1f, duration: 0.2f, vibrato: 1)
            .SetEase(Ease.OutElastic);
            reverseButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
    }

    public void RandomSubmit()
    {
        if (!IsReversed && Random.Range(1, 5) > 3)
        {
            IsReversed = true;
        }
        Card card = hand.RandomRemove();
        submitPosition.Set(card);
        IsSubmitted = true;
        OnSubmitAction?.Invoke();
        hand.ResetPosition();
    }

    public void SetupNextTurn()
    {
        if (IsReversed)
        {
            reversalPosition.DeleteCard();
        }
        submitPosition.DeleteCard();
        IsSubmitted = false;
        if (reverseButton && CanReverse)
        {
            reverseButton?.SetActive(true);
        }
    }

    public void EnemySetSubmitCard(int number,bool isReversed)
    {
        IsReversed = isReversed;
        Card card = hand.Remove(number);
        submitPosition.Set(card);
        IsSubmitted = true;
        OnSubmitAction?.Invoke();
        hand.ResetPosition();
    }

}
