using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;
using UnityEngine.UI;
using UnityEngine.Events;

public class TrashIcon : MonoBehaviour
{
    [SerializeField] GameObject trash;
    public void OnPointerEnter()
    {
        trash.SetActive(true);
        SEManager.Instance.Play(SEPath.POINTDROP);
    }
    public void OnPonterExit()
    {
        trash.SetActive(false);
    }
}
