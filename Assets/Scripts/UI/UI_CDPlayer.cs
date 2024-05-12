using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class UI_CDPlayer : MonoBehaviour
{
    public void ClickToPlayCD(int _cdIndex)
    {
        //唱片机按钮的音效
        AudioManager.instance.PlaySFX(7, null);
        //允许播放cd
        AudioManager.instance.isPlayCD = true;
        //播放指定的cd
        AudioManager.instance.PlayCD(_cdIndex);
    }

    public void ClickToReturnToBGM()
    {
        //由于cd播放完后，我暂时不知道如何判断cd播放完了，故设置一按钮手动恢复播放bgm
        AudioManager.instance.StopAllCD();
        AudioManager.instance.isPlayCD = false;
        AudioManager.instance.isPlayBGM = true;
    }
}