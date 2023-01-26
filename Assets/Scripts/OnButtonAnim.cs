using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class OnButtonAnim : MonoBehaviour
{
    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClickAnim);
    }

    void OnClickAnim()
    {
        transform.DOPunchScale(punch: Vector3.one * 0.1f, duration: 0.2f, vibrato: 1)
            .SetEase(Ease.OutElastic);
    }
}
