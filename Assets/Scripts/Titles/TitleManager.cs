using UnityEngine;
using KanKikuchi.AudioManager;

public class TitleManager : MonoBehaviour
{
    bool onStart;
    public void Start()
    {
        BGMManager.Instance.Play(BGMPath.BGM_TITLE_TENSION);
        BGMManager.Instance.ChangeBaseVolume(0.3f);
    }
    public void OnStartButton()
    {
        if (onStart)
        {
            return;
        }
        onStart = true;
        GameDataManager.Instance.IsOnlineBattle = false;
        BGMManager.Instance.FadeOut();
        FadeManager.Instance.LoadScene("Game", 1f);
    }

    public void OnOnlineStartButton()
    {
        if (onStart)
        {
            return;
        }
        onStart = true;
        GameDataManager.Instance.IsOnlineBattle = true;
        FadeManager.Instance.LoadScene("Online", 1f);
    }

}
