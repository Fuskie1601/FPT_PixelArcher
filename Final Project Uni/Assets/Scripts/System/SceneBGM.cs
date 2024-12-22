using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBGM : MonoBehaviour
{
    public static SceneBGM instance;
    public string BGMStart;
    void Start()
    {
        instance = this;
        ChangeMusic(BGMStart);
    }

    public void ChangeMusic(string BGM)
    {
        //Debug.Log(AudioManager.Instance.playingBGM);
        if(AudioManager.Instance.playingBGM != BGM)
            AudioManager.Instance.ChangeMusic(BGMStart);
    }
}
