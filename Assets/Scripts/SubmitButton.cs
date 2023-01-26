using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SubmitButton : MonoBehaviour
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite OnPointerEnterSprite;
    private void Start()
    {
        //transform.DOScale(0.1f, 1f)
        //    .SetRelative(true)
        //    .SetEase(Ease.OutQuart)
        //    .SetLoops(-1, LoopType.Restart);
    }
    public void OnPointerEnter()
    {
        this.GetComponent<Image>().sprite = OnPointerEnterSprite;
    }
    public void OnPonterExit()
    {
        this.GetComponent<Image>().sprite = defaultSprite;
    }
}
