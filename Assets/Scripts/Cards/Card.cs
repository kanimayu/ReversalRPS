using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using KanKikuchi.AudioManager;


public class Card : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text numberText;
    [SerializeField] Image icon;
    [SerializeField] Image cardFront;
    [SerializeField] Text descriptionText;
    [SerializeField] GameObject hidePanel;

    public CardBase Base { get; private set; }
    public string getName { get => nameText.text; }

    public UnityAction<Card> OnClickCard;

    public void Set(CardBase cardBase, bool isEnemy)
    {
        Base = cardBase;
        nameText.text = cardBase.Name;
        if(cardBase.Level == 5)
        {
            numberText.text = "J";
        }
        else
        {
            numberText.text = cardBase.Level.ToString();
        }
        icon.sprite = cardBase.Icon;
        cardFront.sprite = cardBase.CardFront;
        descriptionText.text = cardBase.Description;
        hidePanel.SetActive(isEnemy);
    }

    public void OnClick()
    {
        OnClickCard?.Invoke(this);
        SEManager.Instance.Play(SEPath.SFX_UI_NEXTPAGE);
    }

    public void OnPointerEnter()
    {
        //transform.position += Vector3.up * 1.3f;
        transform.localScale = Vector3.one * 1.1f;
        GetComponentInChildren<Canvas>().sortingLayerName = "OverLay";
        SEManager.Instance.Play(SEPath.POINTDROP);
    }
    public void OnPonterExit()
    {
        //transform.position -= Vector3.up * 0.3f;
        transform.localScale = Vector3.one;
        GetComponentInChildren<Canvas>().sortingLayerName = "Default";
    }
    public void Open()
    {
        if(hidePanel.activeSelf) StartCoroutine(OpenAnim());
    }
    IEnumerator OpenAnim()
    {
        yield return transform.DORotate(new Vector3(0, 90, 0), 0.2f).WaitForCompletion();
        hidePanel.SetActive(false);
        yield return transform.DORotate(new Vector3(0, 0, 0), 0.2f).WaitForCompletion();
    }
}