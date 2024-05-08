using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Audio_Manager : MonoBehaviour
{
    //静态的全局可调用的实例
    public static Audio_Manager instance;

    #region SFX
    //储存音效（sound effect）的列表
    [SerializeField] AudioSource[] sfx;
    //音效播放的检测半径，太远的音效不予播放
    [SerializeField] float sfxMinPlayRadius;
    #endregion

    #region BGM
    //储存背景音乐的列表
    [SerializeField] AudioSource[] bgm;
    //是否应当（在一定时间内一直）播放某背景音乐
    public bool isPlayBGM;
    //播放的bgm的编号
    public int bgmIndex;
    #endregion

    private void Awake()
    {
        //确保管理器仅有一个
        if(instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    private void Update()
    {
        //管理bgm的播放
        if (!isPlayBGM)
            StopAllBGM();
        else
        {
            //若应当播放的bgm没有播放，开始播放（没有此if会导致一直从头开始播放哦~）
            if (!bgm[bgmIndex].isPlaying)
                PlayBGM(bgmIndex);
        }
    }

    #region SFX
    public void PlaySFX(int _sfxIndex, Transform _sfxSource)
    //传入音效编号以及音效的来源位置，若第二参数填null，则距离对音效播放的限制作废，适用于玩家自己的音效
    {
        //若存在妄图播放的音效但太过遥远，则不播放
        if (_sfxSource != null && Vector2.Distance(PlayerManager.instance.player.transform.position, _sfxSource.position) >= sfxMinPlayRadius)
            return;

        //若编号存在于列表内（编号从0开始哦）
        if(_sfxIndex < sfx.Length)
        {
            //一个小trick,随机化播放目标音效的音高
            sfx[_sfxIndex].pitch = UnityEngine.Random.Range(0.85f, 1.1f);

            //播放音效
            sfx[_sfxIndex].Play();
        }
    }
    //停止音效
    public void StopSFX(int _sfxIndex) => sfx[_sfxIndex].Stop();
    #endregion

    #region BGM
    public void PlayBGM(int _index)
    //传入音效编号
    {
        //若编号存在于列表内（编号从0开始哦）
        if (_index < bgm.Length)
        {
            //bgmIndex才是真正确定当前播放bgm的变量
            bgmIndex = _index;

            //播放前应当先停止播放其他所有背景音乐
            StopAllBGM();
            //播放背景音乐
            bgm[bgmIndex].Play();
        }
    }
    public void StartPlayRandomBGM()
    //随机播放bgm
    {
        bgmIndex = UnityEngine.Random.Range(0, bgm.Length);
        PlayBGM(bgmIndex);
    }
    //关闭所有背景音乐
    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }
    #endregion
}
