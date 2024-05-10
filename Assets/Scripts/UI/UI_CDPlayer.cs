using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class UI_CDPlayer : MonoBehaviour
{
    public void ClickToPlayCD(int _cdIndex)
    {
        //唱片机按钮的音效
        Audio_Manager.instance.PlaySFX(7, null);
        //允许播放cd
        Audio_Manager.instance.isPlayCD = true;
        //播放指定的cd
        Audio_Manager.instance.PlayCD(_cdIndex);
    }

    public void ClickToReturnToBGM()
    {
        //由于cd播放完后，我暂时不知道如何判断cd播放完了，故设置一按钮手动恢复播放bgm
        Audio_Manager.instance.StopAllCD();
        Audio_Manager.instance.isPlayCD = false;
        Audio_Manager.instance.isPlayBGM = true;
    }
}